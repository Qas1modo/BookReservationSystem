using BL.DTOs;

namespace WebAppMVC.Models
{
	public class CartIndexModel
	{
		public IEnumerable<CartItemDetailDto> CartItems { get; set; }
	}
}

