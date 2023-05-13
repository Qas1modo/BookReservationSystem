using DAL;
using DAL.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;

namespace Infrastructure.EFCore.UnitOfWork
{
	public class EFUoWUserInfo : IUoWUserInfo
	{
		public IRepository<User> UserRepository { get; }

		private readonly BookReservationDbContext context;

		public EFUoWUserInfo(BookReservationDbContext context,
			IRepository<User> userRepository)
		{
			this.context = context;
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
