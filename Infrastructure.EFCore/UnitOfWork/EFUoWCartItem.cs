using DAL;
using DAL.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;

namespace Infrastructure.EFCore.UnitOfWork
{
	public class EFUoWCartItem : IUoWCartItem
	{
		public IRepository<CartItem> CartItemRepository { get; }
		public IRepository<Book> BookRepository { get; }
		public IRepository<User> UserRepository { get; }

		private readonly BookReservationDbContext context;

		public EFUoWCartItem(BookReservationDbContext context,
			IRepository<CartItem> cartItemRepository,
			IRepository<Book> bookRepository,
			IRepository<User> userRepository)
		{
			this.context = context;
			this.CartItemRepository = cartItemRepository;
			this.UserRepository = userRepository;
			this.BookRepository = bookRepository;
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
