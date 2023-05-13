using DAL;
using DAL.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;

namespace Infrastructure.EFCore.UnitOfWork
{
	public class EFUoWGenre : IUoWGenre

	{
		public IRepository<Genre> GenreRepository { get; }

		private readonly BookReservationDbContext context;

		public EFUoWGenre(BookReservationDbContext context,
			IRepository<Genre> genreRepository)
		{
			this.context = context;
			this.GenreRepository = genreRepository;
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

