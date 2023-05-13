using DAL;
using DAL.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;

namespace Infrastructure.EFCore.UnitOfWork
{
	public class EFUoWBook : IUoWBook
	{
		public IRepository<Book> BookRepository { get; }
		public IRepository<Genre> GenreRepository { get; }
		public IRepository<Author> AuthorRepository { get; }
		public IRepository<Review> ReviewRepository { get; }

		private readonly BookReservationDbContext context;

		public EFUoWBook(BookReservationDbContext context,
			IRepository<Book> bookRepository,
			IRepository<Genre> genreRepository,
			IRepository<Author> authorRepository,
			IRepository<Review> reviewRepository)
		{
			this.context = context;
			ReviewRepository = reviewRepository;
			BookRepository = bookRepository;
			GenreRepository = genreRepository;
			AuthorRepository = authorRepository;

		}

		public async Task CommitAsync()
		{
			await context.SaveChangesAsync();
		}

		public void Dispose()
		{
			context.Dispose();
		}
	}
}
