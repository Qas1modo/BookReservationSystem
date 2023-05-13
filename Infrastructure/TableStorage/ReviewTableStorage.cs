using AutoMapper;
using Azure.Data.Tables;
using DAL.Constants;
using DAL.Models;
using DAL.TableModels;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.TableStorage
{
	public class ReviewTableStorage
	{
		public readonly TableClient client;
		public readonly string tableName;

		public ReviewTableStorage(string connectionString)
		{
			tableName = Constants.REVIEW_TABLE;
			client = new TableClient(connectionString, tableName);
			client.CreateIfNotExists();
		}

		public async Task<bool> CommitChanges(List<ReviewTable> entityList)
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


		public async Task<bool> FillTables(IEnumerable<Review> reviews,
			IMapper mapper)
		{
			ReviewTable? newItem = null;
			List<ReviewTable> newData = new();
			int count = 0;
			int currentPage = 1;
			foreach (var review in reviews)
			{
				if ((count % Constants.PAGE_SIZE == 0 && count > 0))
				{
					if (!await CommitChanges(newData))
					{
						return false;
					}
					newData = new();
					currentPage++;
					count = 0;
				}
				if ((count > 0 && review.BookId != newItem?.BookId))
				{
					if (!await CommitChanges(newData))
					{
						return false;
					}
					currentPage = 1;
					count = 0;
					newData = new();
				}
				newItem = mapper.Map<ReviewTable>(review);
				newItem.PartitionKey = review.BookId.ToString(Constants.BOOK_FORMAT_STRING) + currentPage.ToString(Constants.PAGE_FORMAT);
				newItem.RowKey = review.Id.ToString(Constants.REVIEW_FORMAT_STRING);
				newItem.AddedAt = newItem.AddedAt.ToUniversalTime();
				newData.Add(newItem);
				count++;
			}
			if (count > 0)
			{
				return await CommitChanges(newData);
			}
			return true;
		}

		public async Task<bool> Resync(IRepository<Review> reviewRepository, IMapper mapper)
		{
			var reviews = (await reviewRepository.GetQueryable().OrderBy(x => x.BookId).ToListAsync());
			return await FillTables(reviews, mapper);
		}
	}
}
