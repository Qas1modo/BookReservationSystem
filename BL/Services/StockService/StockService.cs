using AutoMapper;
using Azure.Data.Tables;
using BL.DTOs;
using BL.Services.PrecomputeService;
using DAL.Constants;
using DAL.Models;
using DAL.TableModels;
using Infrastructure.Query;
using Infrastructure.TableStorage;
using Infrastructure.UnitOfWork;

namespace BL.Services.StockServ
{
	public class StockService : IStockService
	{
		private readonly IMapper mapper;
		private readonly IUoWBook uow;
		private readonly IQuery<Book> query;
		private readonly TableClient _bookClient;
		private readonly TableClient _countClient;
		private readonly IPrecompute _precompute;

		public StockService(IMapper mapper, IUoWBook uow, IQuery<Book> query,
			BookTableStorage bookTable, CountTableStorage countTable, IPrecompute precompute)
		{
			this.mapper = mapper;
			this.uow = uow;
			this.query = query;
			_bookClient = bookTable.client;
			_countClient = countTable.client;
			_precompute = precompute;

		}

		private async Task<QueryResultDto<BookBasicInfoDto>> MaterializedView(BookFilterDto filter)
		{
			string partition;
			if (filter.OnStock)
			{
				if (filter.OrderBy == "Name")
				{
					if (filter.Descending ?? false)
					{
						partition = Constants.STOCK_NAME_DESC_PARTITION;
					}
					else
					{
						partition = Constants.STOCK_NAME_ASC_PARTITION;
					}
				}
				else if (filter.OrderBy == "Price")
				{
					if (filter.Descending ?? false)
					{
						partition = Constants.STOCK_PRICE_DESC_PARTITION;
					}
					else
					{
						partition = Constants.STOCK_PRICE_ASC_PARTITION;
					}
				}
				else
				{
					partition = Constants.STOCK_DEFAULT_PARTITION;
				}
			}
			else
			{
				if (filter.OrderBy == "Name")
				{
					if (filter.Descending ?? true)
					{
						partition = Constants.NOSTOCK_NAME_DESC_PARTITION;
					}
					else
					{
						partition = Constants.NOSTOCK_NAME_ASC_PARTITION;
					}
				}
				else if (filter.OrderBy == "Price")
				{
					if (filter.Descending ?? true)
					{
						partition = Constants.NOSTOCK_PRICE_DESC_PARTITION;
					}
					else
					{
						partition = Constants.NOSTOCK_PRICE_ASC_PARTITION;
					}
				}
				else
				{
					partition = Constants.NOSTOCK_DEFAULT_PARTITION;
				}
			}
			var totalCount = await _countClient.GetEntityAsync<CountTable>(Constants.COUNT_PARTITION, partition);
			List<BookBasicInfoDto> result = new();
			var response = _bookClient.QueryAsync<BookTable>(e => e.PartitionKey == partition + filter.Page.ToString("00000"));
			await foreach (var item in response)
			{
				result.Add(mapper.Map<BookBasicInfoDto>(item));
			}
			return new()
			{
				ItemsCount = totalCount.Value.TotalCount,
				PageNumber = filter.Page,
				PageSize = filter.PageSize ?? Constants.PAGE_SIZE,
				Items = result,
			};
		}

		public async Task<QueryResultDto<BookBasicInfoDto>> ShowBooks(BookFilterDto filter)
		{
			if (filter == null)
			{
				return mapper.Map<QueryResultDto<BookBasicInfoDto>>(await query.Execute());
			}
			if (filter.NameFilter == null && filter.GenreFilter == null &&
				filter.AuthorFilter == null)
			{
				return await MaterializedView(filter);
			}
			query.Where<bool>(a => a == false, "Deleted");
			if (filter.OnStock == true)
			{
				query.Where<int>(a => a > 0, "Stock");
			}
			if (filter.NameFilter != null)
			{
				query.Where<string>(a => a.Contains(filter.NameFilter), "Name");
			}
			if (filter.AuthorFilter != null)
			{
				query.Where<Author>(a => a.Name.Contains(filter.AuthorFilter), "Author");
			}
			if (filter.GenreFilter != null)
			{
				query.Where<Genre>(a => a.Name == filter.GenreFilter, "Genre");
			}
			if (filter.OrderBy != null)
			{
				if (filter.OrderBy == "Price")
				{
					query.OrderBy<decimal>("Price", !filter?.Descending ?? true);
				}
				else if (filter.OrderBy == "Name")
				{
					query.OrderBy<string>("Name", !filter?.Descending ?? true);
				}
			}
			query.Page(filter?.Page ?? 1, filter?.PageSize ?? Constants.PAGE_SIZE);
			return mapper.Map<QueryResultDto<BookBasicInfoDto>>(await query.Execute());
		}

		public async Task<bool> ReserveBookStock(int bookId)
		{
			Book book = await uow.BookRepository.GetByID(bookId);
			if (book.Stock < 1)
			{
				return false;
			}
			if (book.Stock == 1)
			{
				await _precompute.OutOfStock(book);
			}
			return HelperUpdateStock(book, -1);
		}

		public async Task<bool> BookReturnedStock(int bookId)
		{
			Book book = await uow.BookRepository.GetByID(bookId);
			if (book.Stock >= book.Total)
			{
				return false;
			}
			if (book.Stock == 0)
			{
				await _precompute.NowOnStock(book);
			}
			return HelperUpdateStock(book, +1);
		}

		private bool HelperUpdateStock(Book book, int updateStock)
		{
			book.Stock += updateStock;
			uow.BookRepository.Update(book);
			return true;
		}
	}
}
