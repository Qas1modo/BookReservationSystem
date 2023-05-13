using DAL.Models;
using DAL.TableModels;

namespace BL.Services.PrecomputeService
{
	public interface IPrecompute
	{
		public Task<bool> OutOfStock(Book book);
		public Task<bool> NowOnStock(Book book);
		public Task<bool> AddBook(Book book);
		public Task<bool> UpdateBook(Book book, BookTable oldBook);
		public Task<bool> RemoveBook(Book book);
		public Task AddReview(Review newReview, string username);
		public Task DeleteReview(Review review, int page);
		public Task<bool> Resync();
	}
}
