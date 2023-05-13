using DAL;
using DAL.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;

namespace Infrastructure.EFCore.UnitOfWork
{
	public class EFUoWAdmin : IUoWAdmin
	{
		public IRepository<User> UserRepository { get; }

		public IRepository<Book> BookRepository { get; }

		private readonly BookReservationDbContext context;

		public EFUoWAdmin(BookReservationDbContext context,
			IRepository<Book> bookRepository,
			IRepository<User> userRepository)
		{
			this.context = context;
			this.BookRepository = bookRepository;
			this.UserRepository = userRepository;
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
