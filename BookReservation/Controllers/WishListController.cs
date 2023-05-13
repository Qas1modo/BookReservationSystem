using BL.Services.CartItemServ;
using BL.Services.WishListItemService;
using Microsoft.AspNetCore.Mvc;
using WebAppMVC.Models;

namespace WebAppMVC.Controllers
{
	public class WishListController : CommonController
	{

		private readonly IWishListItemService _wishListService;
		private readonly ICartItemService _cartItemService;

		public WishListController(IWishListItemService wishListService,
			ICartItemService cartItemService)
		{
			_wishListService = wishListService;
			_cartItemService = cartItemService;
		}

		[Route("wishlist/DeleteItem/{itemId}")]
		public async Task<IActionResult> DeleteItem(int itemId)
		{
			if (!int.TryParse(User.Identity?.Name, out int userId))
			{
				ModelState.AddModelError("UserId", "Identity error!");
				return RedirectToAction(nameof(Index));
			}
			await _wishListService.DeleteWishlistItem(itemId, userId);
			return RedirectToAction(nameof(Index));
		}

		[HttpGet("wishlist/{page:int?}")]
		public async Task<IActionResult> Index(int page = 1)
		{
			if (page < 1) page = 1;
			int userId = GetValidUser(null);
			var result = await _wishListService.GetWishList(userId, page, 10);
			WishListModel model = new()
			{
				Items = result.Items,
				PageCount = (result.ItemsCount - 1) / result.PageSize + 1,
				PageNumber = result.PageNumber ?? 1,
			};
			return View("Index", model);
		}
	}
}
