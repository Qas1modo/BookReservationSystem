using BL.DTOs;

namespace WebAppMVC.Models
{
	public class WishListModel
	{
		public int PageNumber { get; set; }
		public int PageCount { get; set; }
		public IEnumerable<WishListDetailDto> Items { get; set; }
	}
}
