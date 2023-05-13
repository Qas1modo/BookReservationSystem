using DAL.Models;
using Infrastructure.Repository;

namespace Infrastructure.UnitOfWork
{
	public interface IUoWAdmin : IUnitOfWork
	{
		IRepository<User> UserRepository { get; }

		IRepository<Book> BookRepository { get; }
	}
}
