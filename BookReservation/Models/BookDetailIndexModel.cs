using BL.DTOs;

namespace WebAppMVC.Models
{
	public class BookDetailIndexModel
	{
		public BookBasicInfoDto BookInfo { get; set; }

		public IEnumerable<ReviewDetailDto> Reviews { get; set; }

		public int UserId { get; set; }

		public int PageCount { get; set; }

		public int Page { get; set; }

	}
}

