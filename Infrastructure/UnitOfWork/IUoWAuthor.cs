using DAL.Models;
using Infrastructure.Repository;

namespace Infrastructure.UnitOfWork
{
	public interface IUoWAuthor : IUnitOfWork
	{
		IRepository<Author> AuthorRepository { get; }
	}
}
