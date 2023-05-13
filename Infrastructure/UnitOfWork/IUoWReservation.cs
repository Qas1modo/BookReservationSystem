using DAL.Models;
using Infrastructure.Repository;

namespace Infrastructure.UnitOfWork
{
	public interface IUoWReservation : IUnitOfWork
	{
		IRepository<Reservation> ReservationRepository { get; }

		IRepository<User> UserRepository { get; }
	}
}
