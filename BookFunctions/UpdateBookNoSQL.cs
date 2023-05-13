using System;
using DAL.TableModels;
using Infrastructure.TableStorage;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Azure.Data.Tables;
using DAL.Constants;
using Newtonsoft.Json;
using AutoMapper;

namespace Functions
{
    public class UpdateBookNoSQL
    {

		TableClient _bookClient = new(Environment.GetEnvironmentVariable("NoSqlDb"),
		Constants.BOOK_TABLE);
		TableClient _countClient = new(Environment.GetEnvironmentVariable("NoSqlDb"),
		Constants.COUNT_TABLE);

		[FunctionName("UpdateBookNoSQL")]
        public async Task Run([ServiceBusTrigger("updatebookqueue", Connection = "ServerBus")]string myQueueItem)
        {
			var setting = new JsonSerializerSettings()
			{
				NullValueHandling = NullValueHandling.Ignore,
			};
			BookTableUpdateDto newBook = JsonConvert.DeserializeObject<BookTableUpdateDto>(myQueueItem, setting);
			var mapper = new Mapper(
				new MapperConfiguration(cfg => cfg.CreateMap<BookTableUpdateDto, BookTable>()));
			BookTable oldBook = mapper.Map<BookTable>(newBook);
			BookTable bookTable = mapper.Map<BookTable>(newBook);
			oldBook.Name = newBook.OldName;
			oldBook.Price = newBook.OldPrice;
			if (newBook.OnStock)
				foreach (var partition in HelpFunctions.partitions)
			{
				await HelpFunctions.FindAndReplace(partition, BookTableStorage.GetRowKey(partition, oldBook), bookTable, _bookClient, _countClient);
			}
			foreach (var partition in HelpFunctions.partitionsNoStock)
			{
				await HelpFunctions.FindAndReplace(partition, BookTableStorage.GetRowKey(partition, oldBook), bookTable, _bookClient, _countClient);
			}
		}
    }
}
