using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
	public class BookReservationDbContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Review> Reviews { get; set; }
		public DbSet<CartItem> CartItems { get; set; }
		public DbSet<Reservation> Rents { get; set; }
		public DbSet<Book> Books { get; set; }
		public DbSet<Author> Authors { get; set; }
		public DbSet<Genre> Genres { get; set; }

		public BookReservationDbContext()
		{
		}

		public BookReservationDbContext(DbContextOptions<BookReservationDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Seed();
			base.OnModelCreating(modelBuilder);
		}
	}
}
