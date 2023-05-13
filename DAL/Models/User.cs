using DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
	public class User : BaseEntity
	{
		[StringLength(64), Required]
		public string Name { get; set; }

		[Required, EmailAddress]
		public string Email { get; set; }

		[Required]
		public string Password { get; set; }

		[Required, StringLength(64)]
		public string Salt { get; set; }

		[Required, Phone]
		public string Phone { get; set; }

		[Required]
		public DateTime BirthDate { get; set; }

		[Required, StringLength(64)]
		public string City { get; set; }

		[Required, StringLength(64)]
		public string Street { get; set; }

		[Required, Range(1, 99999)]
		public int StNumber { get; set; }

		[Required, Range(1, 99999)]
		public int ZipCode { get; set; }

		[Required]
		public Group Group { get; set; }

		public virtual List<Reservation> Rents { get; set; }

		public virtual List<Review> Reviews { get; set; }

		public virtual List<CartItem> CartItems { get; set; }

		public virtual List<WishListItem> Wishlist { get; set; }
	}
}
