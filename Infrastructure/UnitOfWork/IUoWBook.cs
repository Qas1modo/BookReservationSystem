using DAL.Models;
using Infrastructure.Repository;


namespace Infrastructure.UnitOfWork
{
	public interface IUoWBook : IUnitOfWork
	{
		IRepository<Book> BookRepository { get; }

		IRepository<Genre> GenreRepository { get; }

		IRepository<Author> AuthorRepository { get; }

		IRepository<Review> ReviewRepository { get; }
	}
}
