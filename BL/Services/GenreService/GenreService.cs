using Azure.Data.Tables;
using BL.DTOs.BasicDtos;
using BL.QueryObjects;
using DAL.Constants;
using DAL.Models;
using DAL.TableModels;
using Infrastructure.TableStorage;
using Infrastructure.UnitOfWork;

namespace BL.Services.GenreServ
{
	public class GenreService : IGenreService
	{
		private readonly IUoWGenre uow;
		private readonly GenreQueryObject queryObject;
		private readonly TableClient _genreClient;

		public GenreService(IUoWGenre uow,
			GenreQueryObject queryObject,
			GenreTableStorage table)
		{
			this.uow = uow;
			this.queryObject = queryObject;
			_genreClient = table.client;
		}

		public async Task<Genre> GetOrAddGenre(string genreName)
		{
			Genre? newGenre = queryObject.GetGenreByName(genreName);
			if (newGenre != null)
			{
				return newGenre;
			}
			uow.GenreRepository.Insert(new Genre() { Name = genreName });
			await uow.CommitAsync();
			newGenre = queryObject.GetExistingGenre(genreName);
			await _genreClient.AddEntityAsync(new TableEntity(Constants.GENRE_PARTITION, newGenre.Id.ToString())
			{
				{ "Name", newGenre.Name },
				{ "Id", newGenre.Id },
			});
			return newGenre;
		}

		public async Task<IEnumerable<GenreDto>> GetAllGenres()
		{
			List<GenreDto> result = new();
			var response = _genreClient.QueryAsync<GenreTable>(e => e.PartitionKey == Constants.GENRE_PARTITION);
			await foreach (var item in response)
			{
				result.Add(new GenreDto
				{
					Name = item.Name,
					Id = item.Id,
				});
			}
			return result;
		}
	}
}
