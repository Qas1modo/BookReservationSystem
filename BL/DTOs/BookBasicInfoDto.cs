using System.ComponentModel.DataAnnotations;

namespace BL.DTOs
{
	public class BookBasicInfoDto
	{
		public int Id { get; set; }

		[Required, StringLength(64)]
		public string Name { get; set; }

		public string Author { get; set; }

		public string Genre { get; set; }

		[Required]
		public bool Deleted { get; set; }

		[StringLength(500)]
		public string? Description { get; set; }

		[Required, Range(0, 999999)]
		public double Price { get; set; }

		public int Total { get; set; }

		[Range(0, int.MaxValue), Required]
		public string Stock { get; set; }
	}
}
