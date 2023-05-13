using System.ComponentModel.DataAnnotations;

namespace BL.DTOs
{
	public class ChangePasswordDto
	{
		public int? UserId { get; set; }
		[Required]
		public string OldPassword { get; set; }
		[Required]
		[StringLength(100, MinimumLength = 8, ErrorMessage = "The new password must be at least 8 characters long!")]
		[RegularExpression(@"([a-zA-Z]+[^a-zA-Z]+|[^a-zA-Z]+[a-zA-Z]+).*", ErrorMessage = "The new password must not contain only letters!")]
		[DataType(DataType.Password)]
		public string NewPassword { get; set; }
		[Required]
		[Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match!")]
		public string ConfirmPassword { get; set; }
	}
}
