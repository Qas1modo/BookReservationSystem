using BL.DTOs;
using BL.DTOs.BasicDtos;

namespace BL.Services.WishListItemService
{
	public interface IWishListItemService
	{
		Task<bool> AddToWishlist(WishListItemDto input);

		Task DeleteWishlistItem(int id,
			int userId = -1,
			bool commit = true);

		Task<QueryResultDto<WishListDetailDto>> GetWishList(int userId,
			int page = 1,
			int pageSize = 10);
	}
}
