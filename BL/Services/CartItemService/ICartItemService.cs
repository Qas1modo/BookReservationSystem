using BL.DTOs;
using BL.DTOs.BasicDtos;

namespace BL.Services.CartItemServ
{
	public interface ICartItemService
	{
		Task AddItem(CartItemDto itemDto);

		Task RemoveItem(int id, int userId = -1, bool commit = true);

		Task EmptyCart(int userId, bool commit = true);

		Task<IEnumerable<CartItemDetailDto>> GetCartItems(int userId);
	}
}

