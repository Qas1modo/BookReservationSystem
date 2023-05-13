using DAL;
using DAL.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;

namespace Infrastructure.EFCore.UnitOfWork
{
	public class EFUoWAuthor : IUoWAuthor
	{
		public IRepository<Author> AuthorRepository { get; }

		private readonly BookReservationDbContext context;

		public EFUoWAuthor(BookReservationDbContext context,
			IRepository<Author> authorRepository)
		{
			this.context = context;
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
