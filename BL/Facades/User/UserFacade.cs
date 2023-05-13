using BL.Services.CartItemServ;
using BL.Services.ReviewServ;
using BL.Services.StockServ;
using BL.Services.WishListItemService;
using DAL.Enums;
using DAL.Models;
using Infrastructure.UnitOfWork;

namespace BL.Facades.UserFac
{
	public class UserFacade : IUserFacade
	{
		private readonly IStockService stockService;
		private readonly ICartItemService cartService;
		private readonly IReviewService reviewService;
		private readonly IWishListItemService wishListItemService;
		private readonly IUoWUser uow;

		public UserFacade(IStockService stockService,
			ICartItemService cartService,
			IReviewService reviewService,
			IWishListItemService wishListItemService,
			IUoWUser uow)
		{
			this.stockService = stockService;
			this.cartService = cartService;
			this.reviewService = reviewService;
			this.wishListItemService = wishListItemService;
			this.uow = uow;
		}

		public void DeleteReservation(Reservation reservation)
		{
			switch (reservation.State)
			{
				case RentState.Reserved:
					stockService.BookReturnedStock(reservation.BookId);
					return;
				case RentState.Active:
					stockService.BookReturnedStock(reservation.BookId);
					return;
			}
			uow.ReservationRepository.Delete(reservation.Id);
		}

		public async Task DeleteUser(int userId)
		{
			User user = await uow.UserRepository.GetByID(userId);
			foreach (var review in user.Reviews)
			{
				await reviewService.DeleteReview(review.Id, commit: false);
			}
			await cartService.EmptyCart(userId, false);
			foreach (var rent in user.Rents)
			{
				DeleteReservation(rent);
			}
			foreach (var wishlistItem in user.Wishlist)
			{
				await wishListItemService.DeleteWishlistItem(wishlistItem.Id);
			}
			uow.UserRepository.Delete(userId);
			await uow.CommitAsync();
		}
	}
}
