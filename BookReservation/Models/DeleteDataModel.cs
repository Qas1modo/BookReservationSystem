using System.ComponentModel.DataAnnotations;

namespace WebAppMVC.Models
{
	public class DeleteDataModel
	{
		[StringLength(64)]
		public string? AuthorName { get; set; }

		[StringLength(64)]
		public string? GenreName { get; set; }
	}
}
