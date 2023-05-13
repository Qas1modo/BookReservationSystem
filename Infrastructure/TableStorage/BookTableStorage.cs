using AutoMapper;
using Azure.Data.Tables;
using DAL.Constants;
using DAL.Models;
using DAL.TableModels;
using Infrastructure.Repository;
using System.Text;

namespace Infrastructure.TableStorage
{
	public class BookTableStorage
	{
		public readonly TableClient client;
		public readonly string tableName;
		private const string alphabet = " !()0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

		public BookTableStorage(string connectionString)
		{
			tableName = Constants.BOOK_TABLE;
			client = new TableClient(connectionString, tableName);
			client.CreateIfNotExists();
		}

		public static (string, List<Book>)[] GetData(IRepository<Book> bookRepository)
		{
			var allBooks = bookRepository.GetQueryable().Where(e => !e.Deleted).ToList();
			var stockBooks = allBooks.Where(e => e.Stock > 0).ToList();
			var stockPriceAsc = stockBooks.OrderBy(e => e.Price).ToList();
			var stockPriceDesc = stockBooks.OrderByDescending(e => e.Price).ToList();
			var stockNameAsc = stockBooks.OrderBy(e => e.Name).ToList();
			var stockNameDesc = stockBooks.OrderByDescending(e => e.Name).ToList();
			var allPriceAsc = allBooks.OrderBy(e => e.Price).ToList();
			var allPriceDesc = allBooks.OrderByDescending(e => e.Price).ToList();
			var allNameAsc = allBooks.OrderBy(e => e.Name).ToList();
			var allNameDesc = allBooks.OrderByDescending(e => e.Name).ToList();
			return new (string, List<Book>)[]
			{
				(Constants.ALL_BOOKS_PARTITION, allBooks),
				(Constants.NOSTOCK_DEFAULT_PARTITION, allBooks),
				(Constants.STOCK_DEFAULT_PARTITION, stockBooks),
				(Constants.STOCK_PRICE_ASC_PARTITION, stockPriceAsc),
				(Constants.STOCK_PRICE_DESC_PARTITION, stockPriceDesc),
				(Constants.STOCK_NAME_ASC_PARTITION, stockNameAsc),
				(Constants.STOCK_NAME_DESC_PARTITION, stockNameDesc),
				(Constants.NOSTOCK_PRICE_ASC_PARTITION, allPriceAsc),
				(Constants.NOSTOCK_PRICE_DESC_PARTITION, allPriceDesc),
				(Constants.NOSTOCK_NAME_ASC_PARTITION, allNameAsc),
				(Constants.NOSTOCK_NAME_DESC_PARTITION, allNameDesc),
			};
		}

		public static string GetRowKey(string partition, BookTable book)
		{
			if (partition == Constants.STOCK_DEFAULT_PARTITION ||
				partition == Constants.NOSTOCK_DEFAULT_PARTITION ||
				partition == Constants.ALL_BOOKS_PARTITION)
			{
				return book.Id.ToString(Constants.BOOK_FORMAT_STRING);
			}
			else if (partition == Constants.STOCK_PRICE_ASC_PARTITION ||
				partition == Constants.NOSTOCK_PRICE_ASC_PARTITION)
			{
				return book.Price.ToString("000000000.00") + book.Id.ToString(Constants.BOOK_FORMAT_STRING);
			}
			else if (partition == Constants.STOCK_PRICE_DESC_PARTITION ||
				partition == Constants.NOSTOCK_PRICE_DESC_PARTITION)
			{
				return (9999999999.99 - (double)book.Price).ToString("000000000.00") + book.Id.ToString(Constants.BOOK_FORMAT_STRING);
			}
			else if (partition == Constants.STOCK_NAME_ASC_PARTITION ||
				partition == Constants.NOSTOCK_NAME_ASC_PARTITION)
			{
				return book.Name.PadRight(64, ' ') + book.Id.ToString(Constants.BOOK_FORMAT_STRING);
			}
			else if (partition == Constants.STOCK_NAME_DESC_PARTITION ||
				partition == Constants.NOSTOCK_NAME_DESC_PARTITION)
			{
				StringBuilder reversedName = new();
				foreach (char character in book.Name)
				{
					int index = alphabet.IndexOf(character);
					if (index == -1) index = alphabet.Length - 1;
					reversedName.Append(alphabet[(alphabet.Length - 1) - index]);
				}
				return reversedName.ToString().PadRight(64, ' ') + book.Id.ToString(Constants.BOOK_FORMAT_STRING);
			}
			return book.Id.ToString(Constants.BOOK_FORMAT_STRING);
		}

		public async Task<bool> CommitChanges(List<BookTable> entityList)
		{
			try
			{
				List<TableTransactionAction> addEntitiesBatch = new();
				addEntitiesBatch.AddRange(entityList.Select(e => new TableTransactionAction(TableTransactionActionType.Add, e)));
				await client.SubmitTransactionAsync(addEntitiesBatch).ConfigureAwait(false);
			}
			catch
			{
				return false;
			}
			return true;
		}

		public static string GetPartition(string partition, int page)
		{
			if (partition == Constants.ALL_BOOKS_PARTITION)
			{
				return partition;
			}
			return partition + page.ToString(Constants.PAGE_FORMAT);
		}


		public async Task<bool> FillTables(string partition,
			IEnumerable<Book> books,
			IMapper mapper)
		{
			BookTable newItem;
			List<BookTable> newData = new();
			int count = 0;
			int currentPage = 1;
			foreach (var book in books)
			{
				if (count % Constants.PAGE_SIZE == 0 && count > 0 &&
					partition != Constants.ALL_BOOKS_PARTITION)
				{
					if (!await CommitChanges(newData))
					{
						return false;
					}
					newData = new();
					currentPage++;
				}
				newItem = mapper.Map<BookTable>(book);
				newItem.PartitionKey = GetPartition(partition, currentPage);
				newItem.RowKey = GetRowKey(partition, newItem);
				newData.Add(newItem);
				count++;
			}
			if (count > 0)
			{
				return await CommitChanges(newData);

			}
			return true;
		}

		public async Task<bool> Resync(IRepository<Book> bookRepository, IMapper mapper)
		{
			var partitions = GetData(bookRepository);
			foreach ((string partition, List<Book> bookList) in partitions)
			{
				if (!await FillTables(partition, bookList, mapper))
				{
					return false;
				}
			}
			return true;
		}
	}
}
