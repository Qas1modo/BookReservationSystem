using System;
using DAL.TableModels;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using DAL.Constants;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Functions;

namespace AddBookNoSQL
{
	public class AddBookNoQSL
	{
		TableClient _bookClient = new(Environment.GetEnvironmentVariable("NoSqlDb"),
			Constants.BOOK_TABLE);
		TableClient _countClient = new(Environment.GetEnvironmentVariable("NoSqlDb"),
		Constants.COUNT_TABLE);

		private async Task AddReviews(int bookId)
		{
			TableEntity newItem = new(Constants.COUNT_REVIEW_PARTITION,
					bookId.ToString(Constants.REVIEW_FORMAT_STRING))
			{
				{"TotalCount", 0 }
			};
			await _countClient.AddEntityAsync(newItem);
		}

		[FunctionName("AddBookNoSQL")]
		public async Task Run([ServiceBusTrigger("addbookqueue", Connection = "ServerBus")] string myQueueItem)
		{
			var setting = new JsonSerializerSettings()
			{
				NullValueHandling = NullValueHandling.Ignore,
			};
			BookTable newBook = JsonConvert.DeserializeObject<BookTable>(myQueueItem, setting);
			if (newBook.OnStock)
			{
				foreach (var partition in HelpFunctions.partitions)
				{
					await HelpFunctions.Insert(partition, newBook, _bookClient, _countClient);
				}
			}
			foreach (var partition in HelpFunctions.partitionsNoStock)
			{
				await HelpFunctions.Insert(partition, newBook, _bookClient, _countClient);
			}
			await AddReviews(newBook.Id);
		}
	}
}
