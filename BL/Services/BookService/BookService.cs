using AutoMapper;
using Azure.Data.Tables;
using BL.DTOs;
using BL.DTOs.BasicDtos;
using BL.Services.PrecomputeService;
using DAL.Constants;
using DAL.Models;
using DAL.TableModels;
using Infrastructure.TableStorage;
using Infrastructure.UnitOfWork;

namespace BL.Services.BookServ
{
	public class BookService : IBookService
	{
		private readonly IMapper mapper;
		private readonly IUoWBook uow;
		private readonly IPrecompute _precompute;
		private readonly TableClient _bookClient;

		public BookService(IUoWBook uow,
			IMapper mapper,
			IPrecompute precompute,
			BookTableStorage bookTable)
		{
			this.uow = uow;
			this.mapper = mapper;
			_precompute = precompute;
			_bookClient = bookTable.client;
		}

		public async Task AddBook(BookDto newBook)
		{
			Book book = mapper.Map<Book>(newBook);
			uow.BookRepository.Insert(book);
			await uow.CommitAsync();
			await _precompute.AddBook(book);
		}

		public async Task<BookBasicInfoDto> GetBook(int bookId)
		{
			var book = (await _bookClient.GetEntityAsync<BookTable>
				(Constants.ALL_BOOKS_PARTITION,
				bookId.ToString(Constants.BOOK_FORMAT_STRING))).Value;
			return mapper.Map<BookBasicInfoDto>(book);
		}
	}
}

