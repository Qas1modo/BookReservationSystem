using BL.DTOs.BasicDtos;
using BL.Facades.BookFac;
using BL.Services.BookServ;
using BL.Services.CartItemServ;
using BL.Services.ReviewServ;
using BL.Services.WishListItemService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAppMVC.Models;

namespace WebAppMVC.Controllers
{
	public class BookDetailController : CommonController
	{
		private readonly IBookService bookService;

		private readonly IReviewService reviewService;

		private readonly ICartItemService cartItemService;

		private readonly IWishListItemService wishlistService;

		private readonly IBookFacade bookFacade;

		public BookDetailController(IBookService bookService,
			IReviewService reviewService,
			ICartItemService cartItemService,
			IBookFacade bookFacade,
			IWishListItemService wishlistService)
		{
			this.bookService = bookService;
			this.reviewService = reviewService;
			this.cartItemService = cartItemService;
			this.bookFacade = bookFacade;
			this.wishlistService = wishlistService;
		}

		private async Task<BookDetailIndexModel> GetModel(int bookId,
			int page,
			int pageSize = 15)
		{
			var reviews = await reviewService.ShowReviews(bookId, page, pageSize);
			if (!int.TryParse(User.Identity?.Name, out int signedUser))
			{
				signedUser = -1;
			}
			return new BookDetailIndexModel()
			{
				BookInfo = await bookService.GetBook(bookId),
				UserId = signedUser,
				Reviews = reviews.Items,
				Page = page,
				PageCount = (reviews.ItemsCount - 1) / reviews.PageSize + 1
			};
		}

		[Route("book/{bookId:int}/{page:int?}")]
		public async Task<IActionResult> Index(int bookId, int page = 1)
		{
			return View(await GetModel(bookId, page));
		}

		[Authorize, HttpPost]
		[Route("book/AddToCart/{bookId:int}/{page:int?}")]
		public async Task<IActionResult> AddToCart([FromForm] AddToCartForm form,
			int bookId,
			int page = 1)
		{
			if (!int.TryParse(User.Identity?.Name, out int userId))
			{
				return RedirectToAction(nameof(Index), new { bookId });
			}

			CartItemDto newItem = new()
			{
				BookId = bookId,
				UserId = userId,
				LoanPeriod = form.days
			};
			await cartItemService.AddItem(newItem);
			return RedirectToAction(nameof(Index), new { bookId, page });
		}

		[Authorize, HttpPost("book/AddReview/{bookId:int}/{page:int?}")]
		public async Task<IActionResult> AddReview([FromForm] ReviewForm form,
			int bookId,
			int page = 1)
		{
			if (!ModelState.IsValid)
			{
				return View("Index", await GetModel(bookId, page));
			}
			if (!int.TryParse(User.Identity?.Name, out int userId))
			{
				ModelState.AddModelError("UserId", "Identity error!");
				return RedirectToAction(nameof(Index), new { bookId });
			}
			ReviewDto newReview = new()
			{
				UserId = userId,
				BookId = bookId,
				Score = form.InlineRadioOptions,
				Description = form.Description
			};

			if (!await reviewService.AddReview(newReview, GetUserName()))
			{
				ModelState.AddModelError("BookId", "You have already wrote the review for this book!");
			}
			return View("Index", await GetModel(bookId, page));
		}

		[Authorize]
		[Route("book/DeleteReview/{bookId:int}/{reviewId:int}/{page:int?}")]
		public async Task<IActionResult> DeleteReview(int bookId,
		int reviewId,
		int page = 1)
		{
			string group = GetGroup();
			int userId;
			if (group == "Admin")
			{
				userId = -1;
			}
			else
			{
				userId = GetValidUser(null);
			}
			if (!await reviewService.DeleteReview(reviewId, userId, page))
			{
				ModelState.AddModelError("ReservationId", "You do not have sufficient permissions");
			}
			return View("Index", await GetModel(bookId, page));
		}

		[Authorize]
		[HttpGet("book/AddToWishlist/{bookId:int}/{page:int?}")]
		public async Task<IActionResult> AddToWishlist(int bookId, int page = 1)
		{
			int userId = GetValidUser(null);
			if (await wishlistService.AddToWishlist(new WishListItemDto { BookId = bookId, UserId = userId }))
			{
				ModelState.AddModelError("UserId", "Book added to wishlist");
			}
			else
			{
				ModelState.AddModelError("UserId", "Book already on wishlist!");
			}
			return View("Index", await GetModel(bookId, page));
		}

		public IActionResult LayoutReservationIndex()
		{
			if (!int.TryParse(User.Identity?.Name, out int userId))
			{
				ModelState.AddModelError("UserId", "Identity error!");
			}
			return Redirect("/reservations/" + userId);
		}
	}
}

