using DAL;
using DAL.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;

namespace Infrastructure.EFCore.UnitOfWork
{
	public class EFUoWUser : IUoWUser
	{
		public IRepository<Reservation> ReservationRepository { get; }
		public IRepository<User> UserRepository { get; }
		public IRepository<Review> ReviewRepository { get; }

		private readonly BookReservationDbContext context;

		public EFUoWUser(BookReservationDbContext context,
			IRepository<Reservation> reservationRepository,
			IRepository<CartItem> cartItemRepository,
			IRepository<Review> reviewRepository,
			IRepository<User> userRepository)
		{
			this.context = context;
			ReservationRepository = reservationRepository;
			UserRepository = userRepository;
			ReviewRepository = reviewRepository;
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
