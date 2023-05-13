using DAL.Models;
using Infrastructure.Repository;

namespace Infrastructure.UnitOfWork
{
	public interface IUoWWishList : IUnitOfWork
	{
		IRepository<WishListItem> WishlistRepository { get; }

		IRepository<User> UserRepository { get; }

		IRepository<Book> BookRepository { get; }
	}
}
