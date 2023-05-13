using System;
using DAL.TableModels;
using Infrastructure.TableStorage;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Newtonsoft.Json;
using DAL.Constants;

namespace Functions
{
    public class BookOutStockNoSQL
    {
		TableClient _bookClient = new(Environment.GetEnvironmentVariable("NoSqlDb"),
		Constants.BOOK_TABLE);
		TableClient _countClient = new(Environment.GetEnvironmentVariable("NoSqlDb"),
		Constants.COUNT_TABLE);

		[FunctionName("BookOutStockNoSQL")]
        public async Task Run([ServiceBusTrigger("bookoutstockqueue", Connection = "ServerBus")]string myQueueItem)
        {
			var setting = new JsonSerializerSettings()
			{
				NullValueHandling = NullValueHandling.Ignore,
			};
			BookTable bookTable = JsonConvert.DeserializeObject<BookTable>(myQueueItem, setting);
			foreach (var partition in HelpFunctions.partitions)
			{
				await HelpFunctions.Remove(partition, bookTable, _bookClient, _countClient);
			}
			foreach (var partition in HelpFunctions.partitionsNoStock)
			{
				await HelpFunctions.FindAndReplace(partition,
					BookTableStorage.GetRowKey(partition, bookTable),
					bookTable, _bookClient, _countClient);
			}
		}
    }
}
