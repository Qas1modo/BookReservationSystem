using BL.DTOs.BasicDtos;
using DAL.Models;

namespace BL.Services.GenreServ
{
	public interface IGenreService
	{
		Task<Genre> GetOrAddGenre(string genreName);

		Task<IEnumerable<GenreDto>> GetAllGenres();
	}
}

