using Azure.Data.Tables;
using DAL.Constants;
using DAL.Models;
using Infrastructure.Repository;

namespace Infrastructure.TableStorage
{
	public class GenreTableStorage
	{
		public readonly TableClient client;
		public readonly string tableName;
		public readonly string defaultPartition;

		public GenreTableStorage(string connectionString)
		{
			tableName = Constants.GENRE_TABLE;
			defaultPartition = Constants.GENRE_PARTITION;
			client = new TableClient(connectionString, tableName);
			client.CreateIfNotExists();
		}

		public async Task<bool> Resync(IRepository<Genre> genreRepository)
		{
			var genres = await genreRepository.GetAll();
			List<TableEntity> entityList = new();
			TableEntity newItem;
			foreach (var genre in genres)
			{
				newItem = new TableEntity(Constants.GENRE_PARTITION, genre.Id.ToString())
				{
					{ "Name", genre.Name },
					{ "Id", genre.Id }
				};
				entityList.Add(newItem);
			}
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
	}
}
