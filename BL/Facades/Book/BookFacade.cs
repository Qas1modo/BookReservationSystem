using AutoMapper;
using Azure.Data.Tables;
using BL.DTOs.BasicDtos;
using BL.Services.AuthorServ;
using BL.Services.BookServ;
using BL.Services.CartItemServ;
using BL.Services.GenreServ;
using BL.Services.PrecomputeService;
using BL.Services.ReservationServ;
using BL.Services.WishListItemService;
using DAL.Constants;
using DAL.Models;
using DAL.TableModels;
using Infrastructure.TableStorage;
using Infrastructure.UnitOfWork;

namespace BL.Facades.BookFac
{
	public class BookFacade : IBookFacade
	{
		private readonly IBookService bookService;

		private readonly IMapper mapper;

		private readonly IAuthorService authorService;

		private readonly IGenreService genreService;

		private readonly IReservationService reservationService;

		private readonly IWishListItemService wishListItemService;

		private readonly ICartItemService cartService;

		private readonly IPrecompute _precompute;

		private readonly IUoWBook uow;

		private readonly TableClient _genreClient;

		public BookFacade(IBookService bookService,
			IAuthorService authorService,
			IGenreService genreService,
			IReservationService reservationService,
			ICartItemService cartService,
			IWishListItemService wishListItemService,
			IUoWBook uow,
			IMapper mapper,
			IPrecompute precompute,
			GenreTableStorage table)
		{
			this.bookService = bookService;
			this.authorService = authorService;
			this.genreService = genreService;
			this.reservationService = reservationService;
			this.cartService = cartService;
			this.uow = uow;
			this.mapper = mapper;
			this.wishListItemService = wishListItemService;
			_genreClient = table.client;
			_precompute = precompute;
		}

		public async Task AddBook(BookDto bookDto)
		{
			bookDto.Author = await authorService.GetOrAddAuthor(bookDto.Author.Name);
			bookDto.Genre = await genreService.GetOrAddGenre(bookDto.Genre.Name);
			await bookService.AddBook(bookDto);
		}


		public async Task<bool> UpdateBook(int bookId, BookDto updatedBook)
		{
			bool nowOnStock = false;
			Book book = await uow.BookRepository.GetByID(bookId);
			BookTable oldBook = mapper.Map<BookTable>(book);
			var newStock = book.Stock + (updatedBook.Total - book.Total);
			if (newStock > 0 && book.Stock <= 0)
			{
				nowOnStock = true;
			}
			if (newStock < 0)
			{
				return false;
			}
			updatedBook.Stock = newStock;
			if (book.Genre.Name != updatedBook.Genre.Name)
			{
				updatedBook.Genre = await genreService.GetOrAddGenre(updatedBook.Genre.Name);
			}
			if (book.Author.Name != updatedBook.Author.Name)
			{
				updatedBook.Author = await authorService.GetOrAddAuthor(updatedBook.Author.Name);
			}
			book = mapper.Map(updatedBook, book);
			uow.BookRepository.Update(book);
			await uow.CommitAsync();
			return await _precompute.UpdateBook(book, oldBook) && (!nowOnStock || await _precompute.NowOnStock(book));
		}

		public async Task<bool> DeleteGenre(string genreName)
		{
			Genre? toRemove = uow.GenreRepository.GetQueryable()
			.Where(x => x.Name == genreName)
			.FirstOrDefault();
			if (toRemove == null)
			{
				return false;
			}
			IEnumerable<Book> books = uow.BookRepository.GetQueryable().Where(r => r.GenreId == toRemove.Id).ToList();
			foreach (Book book in books)
			{
				await DeleteBook(book.Id, false);
			}
			uow.GenreRepository.Delete(toRemove);
			await _genreClient.DeleteEntityAsync(Constants.GENRE_PARTITION, toRemove.Id.ToString());
			await uow.CommitAsync();
			return true;
		}

		public async Task<bool> DeleteAuthor(string authorName)
		{
			Author? toRemove = authorService.GetAuthorByName(authorName);
			if (toRemove == null)
			{
				return false;
			}
			IEnumerable<Book> books = uow.BookRepository.GetQueryable().Where(r => r.AuthorId == toRemove.Id).ToList();
			foreach (var book in books)
			{
				await DeleteBook(book.Id, false);
			}
			uow.AuthorRepository.Delete(toRemove);
			await uow.CommitAsync();
			return true;
		}

		public async Task DeleteBook(int bookId, bool commit = true)
		{
			Book book = await uow.BookRepository.GetByID(bookId);
			foreach (var rent in book.Rents)
			{
				await reservationService.CancelReservation(rent.Id);
			}
			foreach (var cartItem in book.CartItems)
			{
				await cartService.RemoveItem(cartItem.Id, commit: false);
			}
			foreach (var wishlistItem in book.Wishlist)
			{
				await wishListItemService.DeleteWishlistItem(wishlistItem.Id,
					commit: false);
			}
			book.Deleted = true;
			uow.BookRepository.Update(book);
			await _precompute.RemoveBook(book);
			if (commit)
			{
				await uow.CommitAsync();
			}
		}
	}
}

