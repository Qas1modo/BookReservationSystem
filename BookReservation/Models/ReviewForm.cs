using System.ComponentModel.DataAnnotations;

namespace WebAppMVC.Models
{
	public class ReviewForm
	{
		[StringLength(300, ErrorMessage = "Maximum size of message is 300 characters!")]
		public string Description { get; set; }

		[Range(0, 10, ErrorMessage = "Invalid score!"), Required]
		public int InlineRadioOptions { get; set; }
	}
}

