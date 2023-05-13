using System;
using System.Threading.Tasks;
using Functions;
using Azure.Data.Tables;
using DAL.Constants;
using DAL.TableModels;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

namespace RemoveBookNoSQL
{
    public class RemoveBookNoSQL
    {
		TableClient _bookClient = new(Environment.GetEnvironmentVariable("NoSqlDb"),
			Constants.BOOK_TABLE);
		TableClient _countClient = new(Environment.GetEnvironmentVariable("NoSqlDb"),
		Constants.COUNT_TABLE);

		[FunctionName("RemoveBookNoSQL")]
        public async Task Run([ServiceBusTrigger("deletebookqueue", Connection = "ServerBus")]string myQueueItem)
        {
			var setting = new JsonSerializerSettings()
			{
				NullValueHandling = NullValueHandling.Ignore,
			};
			BookTable newBook = JsonConvert.DeserializeObject<BookTable>(myQueueItem, setting);
			foreach (var partition in HelpFunctions.partitions)
			{
				await HelpFunctions.Remove(partition, newBook, _bookClient, _countClient);
			}
			foreach (var partition in HelpFunctions.partitionsNoStock)
			{
				await HelpFunctions.Remove(partition, newBook, _bookClient, _countClient);
			}
		}
    }
}
