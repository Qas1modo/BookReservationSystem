using BL.DTOs;

namespace BL.Services.StockServ
{
	public interface IStockService
	{
		Task<QueryResultDto<BookBasicInfoDto>> ShowBooks(BookFilterDto filter);

		Task<bool> ReserveBookStock(int bookId);

		Task<bool> BookReturnedStock(int bookId);
	}
}
