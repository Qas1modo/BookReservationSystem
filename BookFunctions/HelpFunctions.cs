using AutoMapper;
using Azure.Data.Tables;
using DAL.Constants;
using DAL.TableModels;
using Infrastructure.TableStorage;
using System.Threading.Tasks;

namespace Functions
{
	public class HelpFunctions
	{
		public static readonly string[] partitions = new string[]
			{
				Constants.STOCK_DEFAULT_PARTITION,
				Constants.STOCK_PRICE_ASC_PARTITION,
				Constants.STOCK_PRICE_DESC_PARTITION,
				Constants.STOCK_NAME_ASC_PARTITION,
				Constants.STOCK_NAME_DESC_PARTITION,
			};
		public static readonly string[] partitionsNoStock = new string[]
		{
				Constants.ALL_BOOKS_PARTITION,
				Constants.NOSTOCK_DEFAULT_PARTITION,
				Constants.NOSTOCK_PRICE_ASC_PARTITION,
				Constants.NOSTOCK_PRICE_DESC_PARTITION,
				Constants.NOSTOCK_NAME_ASC_PARTITION,
				Constants.NOSTOCK_NAME_DESC_PARTITION,
		};

		public static async Task<BookTable> InsertInside(string partition, BookTable book, int totalCount,
						TableClient _bookClient, bool makeOnStock = false)
		{
			if (makeOnStock) book.OnStock = true;
			int count;
			int page = 1;
			bool foundPlace = false;
			string rowKey = BookTableStorage.GetRowKey(partition, book);
			for (; page * Constants.PAGE_SIZE <= totalCount; page++)
			{
				var response = _bookClient.QueryAsync<BookTable>(e => e.PartitionKey == BookTableStorage.GetPartition(partition, page));
				count = 1;
				await foreach (var item in response)
				{
					if (foundPlace)
					{
						if (count >= Constants.PAGE_SIZE)
						{
							await _bookClient.DeleteEntityAsync(item.PartitionKey, item.RowKey);
							await _bookClient.AddEntityAsync(book);
							book = item;
							book.PartitionKey = BookTableStorage.GetPartition(partition, page + 1);
							break;
						}
					}
					else if (item.RowKey.CompareTo(rowKey) >= 0)
					{
						foundPlace = true;
						book.PartitionKey = BookTableStorage.GetPartition(partition, page);
						book.RowKey = BookTableStorage.GetRowKey(partition, book);
						if (count >= Constants.PAGE_SIZE)
						{
							await _bookClient.DeleteEntityAsync(item.PartitionKey, item.RowKey);
							await _bookClient.AddEntityAsync(book);
							book = item;
							book.PartitionKey = BookTableStorage.GetPartition(partition, page + 1);
							break;
						}
					}
					count++;
				}
			}
			if (!foundPlace)
			{
				book.PartitionKey = BookTableStorage.GetPartition(partition, page);
				book.RowKey = BookTableStorage.GetRowKey(partition, book);
			}
			return book;
		}

		public static async Task Insert(string partition, BookTable book, 
			TableClient _bookClient, TableClient _countClient, bool makeOnStock = false)
		{
			var totalCount = _countClient.
				GetEntityAsync<CountTable>(Constants.COUNT_PARTITION, partition).Result.Value;
			TableEntity newItem = new(Constants.COUNT_PARTITION,
				partition)
			{
				{"TotalCount", totalCount.TotalCount + 1}
			};
			await _countClient.UpdateEntityAsync(newItem, totalCount.ETag);
			if (partition == Constants.STOCK_DEFAULT_PARTITION ||
				partition == Constants.NOSTOCK_DEFAULT_PARTITION ||
				partition == Constants.ALL_BOOKS_PARTITION)
			{
				book.PartitionKey = BookTableStorage.GetPartition(partition, totalCount.TotalCount / Constants.PAGE_SIZE + 1);
				book.RowKey = book.Id.ToString(Constants.BOOK_FORMAT_STRING);
				book.OnStock = makeOnStock || book.OnStock;
			}
			else
			{
				book = await InsertInside(partition, book, totalCount.TotalCount, _bookClient, makeOnStock);
			}
			await _bookClient.AddEntityAsync(book);
		}

		public static async Task Remove(string partition, BookTable book,
			TableClient _bookClient, TableClient _countClient)
		{
			var totalCount = _countClient.
				GetEntityAsync<CountTable>(Constants.COUNT_PARTITION, partition).Result.Value;
			book.RowKey = BookTableStorage.GetRowKey(partition, book);
			BookTable? toInsert = null;
			BookTable? firstItem;
			for (int page = totalCount.TotalCount / Constants.PAGE_SIZE + 1; page != 0; page--)
			{
				firstItem = null;
				var response = _bookClient.QueryAsync<BookTable>(e => e.PartitionKey == BookTableStorage.GetPartition(partition, page));
				await foreach (var item in response)
				{
					firstItem ??= item;
					if (item.RowKey == book.RowKey)
					{
						if (toInsert is not null)
						{
							var test = await _bookClient.AddEntityAsync(toInsert);
						}
						await _bookClient.DeleteEntityAsync(item.PartitionKey, item.RowKey);
						TableEntity newItem = new(Constants.COUNT_PARTITION,
						partition)
						{
							{"TotalCount", totalCount.TotalCount - 1}
						};
						await _countClient.UpdateEntityAsync(newItem, totalCount.ETag);
						return;
					}
				}
				if (firstItem is null) return;
				if (toInsert is not null)
				{
					await _bookClient.AddEntityAsync(toInsert);
				}
				toInsert = firstItem;
				await _bookClient.DeleteEntityAsync(firstItem.PartitionKey, firstItem.RowKey);
				toInsert.PartitionKey = BookTableStorage.GetPartition(partition, page - 1);
			}
		}


		public static async Task FindAndReplace(string partition,
			string RowKey, BookTable table, TableClient _bookClient,
			TableClient _countClient)
		{
			var mapper = new Mapper(
				new MapperConfiguration(cfg => cfg.CreateMap<BookTable, BookTable>()
				.ForMember(dest => dest.Timestamp, cfg => cfg.Ignore())
				.ForMember(dest => dest.ETag, cfg => cfg.Ignore())
				.ForMember(dest => dest.PartitionKey, cfg => cfg.Ignore())
				.ForMember(dest => dest.RowKey, cfg => cfg.Ignore())));
			int totalCount = _countClient.
			   GetEntityAsync<CountTable>(Constants.COUNT_PARTITION, partition).Result.Value.TotalCount;
			for (int page = totalCount / Constants.PAGE_SIZE + 1; page != 0; page--)
			{
				var response = _bookClient.QueryAsync<BookTable>(e => e.PartitionKey == BookTableStorage.GetPartition(partition, page));
				await foreach (var item in response)
				{
					if (item.RowKey == RowKey)
					{
						await _bookClient.UpdateEntityAsync(mapper.Map(table, item), item.ETag);
						break;
					}
				}
			}
		}

	}
}
