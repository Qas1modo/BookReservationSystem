using System.ComponentModel.DataAnnotations;

namespace WebAppMVC.Models
{
	public class AdminPageBookModel
	{
		[StringLength(64)]
		public string Name { get; set; }

		[StringLength(64)]
		public string Author { get; set; }

		[StringLength(64)]
		public string Genre { get; set; }

		[StringLength(64)]
		public string? NewGenreName { get; set; }

		[Range(0, int.MaxValue)]
		public int Total { get; set; }

		[Required, Range(0, 999999)]
		public decimal Price { get; set; }

		[StringLength(500)]
		public string? Description { get; set; }
	}
}
