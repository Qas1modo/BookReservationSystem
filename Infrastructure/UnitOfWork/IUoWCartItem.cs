using DAL.Models;
using Infrastructure.Repository;

namespace Infrastructure.UnitOfWork
{
	public interface IUoWCartItem : IUnitOfWork
	{
		IRepository<User> UserRepository { get; }
		IRepository<Book> BookRepository { get; }
		IRepository<CartItem> CartItemRepository { get; }
	}
}
