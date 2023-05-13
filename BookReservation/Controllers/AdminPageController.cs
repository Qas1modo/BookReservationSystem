using AutoMapper;
using BL.DTOs;
using BL.DTOs.BasicDtos;
using BL.Facades.BookFac;
using BL.Facades.OrderFac;
using BL.Facades.UserFac;
using BL.Services.AuthorServ;
using BL.Services.BookServ;
using BL.Services.GenreServ;
using BL.Services.PrecomputeService;
using BL.Services.StockServ;
using BL.Services.UserServ;
using DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppMVC.Models;

namespace WebAppMVC.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminPageController : CommonController
	{

		private readonly IUserService userService;
		private readonly IStockService stockService;
		private readonly IBookService bookService;
		private readonly IGenreService genreService;
		private readonly IBookFacade bookFacade;
		private readonly IUserFacade userFacade;
		private readonly IOrderFacade orderFacade;
		private readonly IAuthorService authorService;
		private readonly IMapper mapper;
		private readonly IPrecompute precompute;

		public AdminPageController(IUserService userService,
			IStockService stockService,
			IBookService bookService,
			IGenreService genreService,
			IBookFacade bookFacade,
			IMapper mapper,
			IAuthorService authorService,
			IOrderFacade orderFacade,
			IPrecompute precomputeGenre,
			IUserFacade userFacade)
		{
			this.userService = userService;
			this.stockService = stockService;
			this.bookService = bookService;
			this.genreService = genreService;
			this.bookFacade = bookFacade;
			this.mapper = mapper;
			this.userFacade = userFacade;
			this.authorService = authorService;
			this.orderFacade = orderFacade;
			this.precompute = precomputeGenre;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet("AdminPage/Users/{page:int?}")]
		public async Task<IActionResult> Users(int page = 1)
		{
			page = page < 1 ? 1 : page;
			var result = await userService.ShowUsers(page);
			var model = new AdminPageUsersModel
			{
				Users = result.Items,
				Page = page,
				Total = (result.ItemsCount - 1) / result.PageSize + 1,
				SignedUser = GetValidUser(null),
			};
			return View(model);
		}

		[HttpGet("AdminPage/Books/{page:int?}")]
		public async Task<IActionResult> Books(int page = 1)
		{

			var model = new AdminPageBooksModel();
			BookFilterDto bookFilter = new()
			{
				Page = page < 1 ? 1 : page,
				PageSize = 10
			};
			var serviceResult = await stockService.ShowBooks(bookFilter);

			model.Books = serviceResult.Items;
			model.Page = page;
			model.Total = (serviceResult.ItemsCount - 1) / serviceResult.PageSize + 1;

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> ChangeBookInfo(int id)
		{
			await GetGenreList();
			BookBasicInfoDto book = await bookService.GetBook(id);
			if (book == null)
			{
				return RedirectToAction(nameof(Books));
			}
			return View("ChangeBookInfo", mapper.Map<AdminPageBookModel>(book));

		}

		[HttpGet]
		public async Task<IActionResult> DeleteData()
		{
			await GetGenreList();
			return View("DeleteData");

		}

		[HttpPost]
		public async Task<IActionResult> DeleteData(DeleteDataModel model)
		{
			if (model.GenreName != null)
			{
				if (await bookFacade.DeleteGenre(model.GenreName))
				{
					ModelState.AddModelError("GenreName", "Genre Removed");
				}
				else
				{
					ModelState.AddModelError("GenreName", "Genre invalid!");
				}
			}
			else if (model.AuthorName != null)
			{
				if (await bookFacade.DeleteAuthor(model.AuthorName))
				{
					ModelState.AddModelError("AuthorName", "Author Removed");
				}
				else
				{
					ModelState.AddModelError("GenreName", "Author not found!");
				}
			}
			await GetGenreList();
			return View("DeleteData", model);
		}

		private async Task GetGenreList()
		{
			var genres = await genreService.GetAllGenres();
			SelectList dropDownItems = new SelectList(genres.Select(x => new KeyValuePair<string, string>(x.Name, x.Name)), "Key", "Value");
			ViewBag.genres = dropDownItems;
		}

		[HttpPost]
		public async Task<IActionResult> ChangeBookInfo(int id, AdminPageBookModel model)
		{
			await GetGenreList();
			if (!ModelState.IsValid)
			{
				return View("ChangeBookInfo", model);
			}
			if (model.NewGenreName != null)
			{
				model.Genre = model.NewGenreName;
			}
			if (await bookFacade.UpdateBook(id, mapper.Map<BookDto>(model)))
			{
				ModelState.AddModelError("Name", "Changes applied succesfully!");
			}
			else
			{
				ModelState.AddModelError("AuthorName", "Total too low or other error occured!");
			}
			return View("ChangeBookInfo", model);
		}

		public async Task<IActionResult> AddBook()
		{
			await GetGenreList();
			return View(new AdminPageBookModel());
		}

		[HttpPost]
		public async Task<IActionResult> AddBook(AdminPageBookModel model)
		{
			await GetGenreList();
			if (!ModelState.IsValid)
			{
				return View("AddBook", model);
			}
			if (model.NewGenreName != null)
			{
				model.Genre = model.NewGenreName;
			}
			BookDto bookDto = mapper.Map<BookDto>(model);
			await bookFacade.AddBook(bookDto);
			ModelState.AddModelError("Name", "Book created succesfully!");
			return View("AddBook", model);
		}

		[HttpPost]
		public async Task<IActionResult> ChangeGroup(int userId, string newgroup)
		{
			if (GetValidUser(null) == userId)
			{
				return RedirectToAction("Users");
			}
			if (!Enum.TryParse(newgroup, true, out Group group))
			{
				return RedirectToAction("Users");
			}
			await userService.UpdateUserPermission(userId, group);
			return RedirectToAction("Users");
		}

		public async Task<IActionResult> Deleteuser(int userId)
		{
			await userFacade.DeleteUser(userId);
			return RedirectToAction("Users");
		}

		public async Task<IActionResult> DeleteBook(int bookId)
		{
			await bookFacade.DeleteBook(bookId);
			return Redirect("Books");
		}

		public async Task<IActionResult> ExpireOldReservations()
		{
			await orderFacade.ExpireOldReservations();
			ModelState.AddModelError("Id", "Reserations manually expired!");
			return View("Index");
		}

		public async Task<IActionResult> Resync()
		{
			if (await precompute.Resync())
			{
				ModelState.AddModelError("Id", "Resync successfull!");
			}
			else
			{
				ModelState.AddModelError("Id", "Resync failed!");
			}
			return View("Index");
		}
	}
}
