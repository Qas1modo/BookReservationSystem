using BL.DTOs;
using BL.DTOs.BasicDtos;

namespace BL.Services.BookServ
{
	public interface IBookService
	{
		Task AddBook(BookDto newBook);

		Task<BookBasicInfoDto> GetBook(int bookId);
	}
}

