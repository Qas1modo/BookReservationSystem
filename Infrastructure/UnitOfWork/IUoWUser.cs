using DAL.Models;
using Infrastructure.Repository;

namespace Infrastructure.UnitOfWork
{
	public interface IUoWUser : IUnitOfWork
	{
		IRepository<User> UserRepository { get; }
		IRepository<Review> ReviewRepository { get; }
		IRepository<Reservation> ReservationRepository { get; }
	}
}
