using System;
using DAL.TableModels;
using Infrastructure.TableStorage;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Azure.Data.Tables;
using DAL.Constants;

namespace Functions
{
    public class BookOnStockNoSQL
    {
		TableClient _bookClient = new(Environment.GetEnvironmentVariable("NoSqlDb"),
			Constants.BOOK_TABLE);
		TableClient _countClient = new(Environment.GetEnvironmentVariable("NoSqlDb"),
		Constants.COUNT_TABLE);

		[FunctionName("BookOnStockNoSQL")]
        public async Task Run([ServiceBusTrigger("bookonstockqueue", Connection = "ServerBus")]string myQueueItem)
        {
			var setting = new JsonSerializerSettings()
			{
				NullValueHandling = NullValueHandling.Ignore,
			};
			BookTable bookTable = JsonConvert.DeserializeObject<BookTable>(myQueueItem, setting);
			foreach (var partition in HelpFunctions.partitions)
			{
				await HelpFunctions.Insert(partition, bookTable, _bookClient, _countClient, true);
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
