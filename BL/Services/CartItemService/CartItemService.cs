using AutoMapper;
using BL.DTOs;
using BL.DTOs.BasicDtos;
using DAL.Models;
using Infrastructure.UnitOfWork;

namespace BL.Services.CartItemServ
{
	public class CartItemService : ICartItemService
	{
		private readonly IMapper mapper;
		private readonly IUoWCartItem uow;

		public CartItemService(IUoWCartItem uow, IMapper mapper)
		{
			this.mapper = mapper;
			this.uow = uow;
		}

		public async Task AddItem(CartItemDto itemDto)
		{
			if ((await uow.BookRepository.GetByID(itemDto.BookId)).Deleted)
			{
				return;
			}
			uow.CartItemRepository.Insert(mapper.Map<CartItem>(itemDto));
			await uow.CommitAsync();
		}

		public async Task RemoveItem(int id, int userId = -1, bool commit = true)
		{
			CartItem item = await uow.CartItemRepository.GetByID(id);
			if (item.UserId != userId && userId != -1)
			{
				return;
			}
			uow.CartItemRepository.Delete(item);
			if (commit) await uow.CommitAsync();
		}

		public async Task EmptyCart(int userId, bool commit = true)
		{
			User user = await uow.UserRepository.GetByID(userId);
			foreach (var cartItem in user.CartItems)
			{
				uow.CartItemRepository.Delete(cartItem.Id);
			}
			if (commit) await uow.CommitAsync();
		}

		public async Task<IEnumerable<CartItemDetailDto>> GetCartItems(int userId)
		{
			User user = await uow.UserRepository.GetByID(userId);
			return mapper.Map<IEnumerable<CartItemDetailDto>>(user.CartItems);
		}
	}
}

