using DAL.Models;
using Infrastructure.Repository;

namespace Infrastructure.UnitOfWork
{
	public interface IUoWGenre : IUnitOfWork
	{
		IRepository<Genre> GenreRepository { get; }
	}
}

