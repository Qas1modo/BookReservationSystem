using BL.DTOs;

namespace WebAppMVC.Models
{
	public class AdminPageBooksModel
	{
		public IEnumerable<BookBasicInfoDto> Books { get; set; }

		public int Page { get; set; }

		public int Total { get; set; }
	}
}
