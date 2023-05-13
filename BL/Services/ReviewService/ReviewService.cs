using AutoMapper;
using Azure;
using Azure.Data.Tables;
using BL.DTOs;
using BL.DTOs.BasicDtos;
using BL.Services.PrecomputeService;
using DAL.Constants;
using DAL.Models;
using DAL.TableModels;
using Infrastructure.TableStorage;
using Infrastructure.UnitOfWork;

namespace BL.Services.ReviewServ
{
	public class ReviewService : IReviewService
	{
		private readonly IMapper mapper;
		private readonly IUoWReview uow;
		private readonly TableClient _countClient;
		private readonly TableClient _reviewClient;
		private readonly IPrecompute _precompute;

		public ReviewService(IUoWReview uow,
			IMapper mapper,
			IPrecompute precompute,
			CountTableStorage countTable,
			ReviewTableStorage reviewTable)
		{
			this.mapper = mapper;
			this.uow = uow;
			_countClient = countTable.client;
			_reviewClient = reviewTable.client;
			_precompute = precompute;

		}

		public async Task<bool> AddReview(ReviewDto reviewDto, string username)
		{
			int reviewCount = uow.ReviewRepository.GetQueryable()
				.Where(x => x.UserId == reviewDto.UserId)
				.Where(x => x.BookId == reviewDto.BookId)
				.Count();
			if (reviewCount > 0)
			{
				return false;
			}
			reviewDto.AddedAt = DateTime.Now;
			var newReview = mapper.Map<Review>(reviewDto);
			uow.ReviewRepository.Insert(newReview);
			await uow.CommitAsync();
			await _precompute.AddReview(newReview, username);
			return true;
		}

		public async Task<bool> DeleteReview(int reviewId, int userId = -1, int page = -1, bool commit = true)
		{
			Review review = await uow.ReviewRepository.GetByID(reviewId);
			if (review.UserId != userId && userId != -1)
			{
				return false;
			}
			uow.ReviewRepository.Delete(reviewId);
			if (commit) await uow.CommitAsync();
			await _precompute.DeleteReview(review, page);
			return true;
		}

		public async Task<QueryResultDto<ReviewDetailDto>> ShowReviews(int bookId, int page, int pageSize)
		{
			int reviewCount;
			List<ReviewDetailDto> result = new();
			try
			{
				reviewCount = (await _countClient.GetEntityAsync<CountTable>(Constants.COUNT_REVIEW_PARTITION,
				bookId.ToString(Constants.REVIEW_FORMAT_STRING))).Value.TotalCount;
				var response = _reviewClient.QueryAsync<ReviewTable>
					(e => e.PartitionKey == bookId.ToString(Constants.BOOK_FORMAT_STRING) + page.ToString(Constants.PAGE_FORMAT));
				await foreach (var item in response)
				{
					result.Add(mapper.Map<ReviewDetailDto>(item));
				}
			}
			catch (RequestFailedException ex)
			{
				reviewCount = 0;
				throw ex;
			}
			return new()
			{
				ItemsCount = reviewCount,
				PageNumber = page,
				PageSize = pageSize,
				Items = result,
			};
		}
	}
}
