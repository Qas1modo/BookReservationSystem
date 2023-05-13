using DAL;
using DAL.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;

namespace Infrastructure.EFCore.UnitOfWork
{
	public class EFUoWReservation : IUoWReservation
	{
		public IRepository<Reservation> ReservationRepository { get; }
		public IRepository<User> UserRepository { get; }

		private readonly BookReservationDbContext context;

		public EFUoWReservation(BookReservationDbContext context,
			IRepository<Reservation> reservationRepository,
			IRepository<User> userRepository)
		{
			this.context = context;
			this.ReservationRepository = reservationRepository;
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
