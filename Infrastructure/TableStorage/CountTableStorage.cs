using Azure.Data.Tables;
using DAL.Constants;
using DAL.Models;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.TableStorage
{
	public class CountTableStorage
	{
		public readonly TableClient client;
		public readonly string tableName;

		public CountTableStorage(string connectionString)
		{
			tableName = Constants.COUNT_TABLE;
			client = new TableClient(connectionString, tableName);
			client.CreateIfNotExists();
		}

		private async Task<bool> CommitChanges(List<TableEntity> entityList)
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

		public async Task<bool> Resync(IRepository<Book> bookRepository, IRepository<Review> reviewRepository)
		{
			var partitions = BookTableStorage.GetData(bookRepository);
			var reviews = await reviewRepository.GetQueryable().GroupBy(x => x.BookId)
				.Select(group => new { group.Key, Count = group.Count() })
				.ToDictionaryAsync(x => x.Key, x => x.Count);
			List<TableEntity> entityList = new();
			foreach (var partition in partitions)
			{
				TableEntity newItem = new(Constants.COUNT_PARTITION, partition.Item1)
			{
				{"TotalCount", partition.Item2.Count}
			};
				entityList.Add(newItem);
			}
			if (!await CommitChanges(entityList)) return false;
			entityList = new();
			foreach (Book book in partitions[0].Item2)
			{
				TableEntity newItem = new(Constants.COUNT_REVIEW_PARTITION,
					book.Id.ToString(Constants.REVIEW_FORMAT_STRING))
			{
				{"TotalCount", reviews.GetValueOrDefault(book.Id, 0) }
			};
				entityList.Add(newItem);
			}
			return await CommitChanges(entityList);
		}
	}
}