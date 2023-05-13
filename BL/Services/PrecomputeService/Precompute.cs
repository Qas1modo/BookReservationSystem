using AutoMapper;
using Azure.Data.Tables;
using Azure.Messaging.ServiceBus;
using DAL.Constants;
using DAL.Models;
using DAL.TableModels;
using Infrastructure.TableStorage;
using Infrastructure.UnitOfWork;
using System.Text.Json;

namespace BL.Services.PrecomputeService
{
	public class Precompute : IPrecompute
	{
		private readonly IUoWBook _uowBook;
		private readonly GenreTableStorage _genreTable;
		private readonly BookTableStorage _bookTable;
		private readonly ReviewTableStorage _reviewTable;
		private readonly CountTableStorage _countTable;
		private readonly ServiceBusClient _busClient;
		private readonly IMapper _mapper;
		public Precompute(IUoWBook bookUow,
			GenreTableStorage genreTable,
			BookTableStorage bookTable,
			ReviewTableStorage reviewTable,
			CountTableStorage countTable,
			ServiceBusClient serviceBusClient,
			IMapper mapper)
		{
			_uowBook = bookUow;
			_genreTable = genreTable;
			_bookTable = bookTable;
			_mapper = mapper;
			_reviewTable = reviewTable;
			_countTable = countTable;
			_busClient = serviceBusClient;
		}

		public async Task<bool> OutOfStock(Book book)
		{
			BookTable bookTable = _mapper.Map<BookTable>(book);
			bookTable.OnStock = false;
			var sender = _busClient.CreateSender("bookoutstockqueue");
			await sender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(bookTable)));
			return true;
		}


		public async Task<bool> NowOnStock(Book book)
		{
			BookTable bookTable = _mapper.Map<BookTable>(book);
			bookTable.OnStock = false;
			var sender = _busClient.CreateSender("bookonstockqueue");
			await sender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(bookTable)));
			return true;
		}

		public async Task<bool> AddBook(Book book)
		{
			BookTable bookTable = _mapper.Map<BookTable>(book);
			var sender = _busClient.CreateSender("addbookqueue");
			await sender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(bookTable)));
			return true;
		}

		public async Task<bool> RemoveBook(Book book)
		{
			BookTable bookTable = _mapper.Map<BookTable>(book);
			var sender = _busClient.CreateSender("deletebookqueue");
			await sender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(bookTable)));
			return true;
		}

		public async Task<bool> UpdateBook(Book book, BookTable oldBook)
		{
			BookTableUpdateDto updatedBook = _mapper.Map<BookTableUpdateDto>(book);
			updatedBook.OldName = oldBook.Name;
			updatedBook.OldPrice = oldBook.Price;
			var sender = _busClient.CreateSender("updatebookqueue");
			await sender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(updatedBook)));
			return true;
		}

		public async Task<CountTable> UpdateCountReview(Review review, bool increase = false, int tries = 3)
		{
			while (true)
			{
				CountTable reviewCount;
				try
				{
					reviewCount = (await _countTable.client.GetEntityAsync<CountTable>(Constants.COUNT_REVIEW_PARTITION,
					review.BookId.ToString(Constants.REVIEW_FORMAT_STRING))).Value;
					TableEntity newItem = new(Constants.COUNT_REVIEW_PARTITION,
					review.BookId.ToString(Constants.REVIEW_FORMAT_STRING))
					{
						{"TotalCount", reviewCount.TotalCount + (increase ? 1 : -1)}
					};
					await _countTable.client.UpdateEntityAsync(newItem, reviewCount.ETag);
				}
				catch (Exception e)
				{
					if (tries <= 0) throw e;
					tries--;
					continue;
				}
				return reviewCount;
			}
		}

		public async Task DeleteReview(Review review, int page)
		{
			CountTable reviewCount = await UpdateCountReview(review);
			if (page == -1)
			{
				for (int i = 0; i * Constants.PAGE_SIZE <= reviewCount.TotalCount; i++)
				{
					var response = _reviewTable.client.QueryAsync<ReviewTable>
					(e => e.PartitionKey == review.BookId.ToString(Constants.BOOK_FORMAT_STRING) +
					i.ToString(Constants.PAGE_FORMAT));
					await foreach (var row in response)
					{
						if (row.Id == review.Id)
						{
							var resp = await _reviewTable.client.DeleteEntityAsync(
								review.BookId.ToString(Constants.BOOK_FORMAT_STRING) + i.ToString(Constants.PAGE_FORMAT),
								review.Id.ToString(Constants.REVIEW_FORMAT_STRING));
						}
					}
				}
				return;
			}
			await _reviewTable.client.DeleteEntityAsync(review.BookId.ToString(Constants.BOOK_FORMAT_STRING) + page.ToString(Constants.PAGE_FORMAT),
							review.Id.ToString(Constants.REVIEW_FORMAT_STRING));
		}

		public async Task AddReview(Review newReview, string username)
		{
			var totalCount = await UpdateCountReview(newReview, true);
			int page = totalCount.TotalCount / Constants.PAGE_SIZE + 1;
			var newItem = _mapper.Map<ReviewTable>(newReview);
			newItem.PartitionKey = newReview.BookId.ToString(Constants.BOOK_FORMAT_STRING) + page.ToString(Constants.PAGE_FORMAT);
			newItem.RowKey = newReview.Id.ToString(Constants.REVIEW_FORMAT_STRING);
			newItem.User = username;
			await _reviewTable.client.AddEntityAsync(newItem);
		}

		public async Task<bool> Resync()
		{
			await _genreTable.client.DeleteAsync();
			await _bookTable.client.DeleteAsync();
			await _reviewTable.client.DeleteAsync();
			await _countTable.client.DeleteAsync();
			while (true)
			{
				try
				{
					await _genreTable.client.CreateIfNotExistsAsync();
					await _bookTable.client.CreateIfNotExistsAsync();
					await _reviewTable.client.CreateIfNotExistsAsync();
					await _countTable.client.CreateIfNotExistsAsync();
				}
				catch
				{
					Thread.Sleep(1500);
					continue;
				}
				break;
			}
			return await _genreTable.Resync(_uowBook.GenreRepository) &&
				await _bookTable.Resync(_uowBook.BookRepository, _mapper) &&
				await _reviewTable.Resync(_uowBook.ReviewRepository, _mapper) &&
				await _countTable.Resync(_uowBook.BookRepository, _uowBook.ReviewRepository);
		}
	}
}
