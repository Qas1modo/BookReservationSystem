using System.ComponentModel.DataAnnotations;

namespace BL.DTOs
{
	public class PersonalInfoDto
	{
		public int? Id { get; set; }

		[Required, StringLength(64, ErrorMessage = "Username name too long!")]
		[MinLength(3, ErrorMessage = "Username must be at least 3 characters long!")]
		public string Name { get; set; }

		[Required, StringLength(64, ErrorMessage = "City name too long!")]
		[MinLength(3, ErrorMessage = "City must be at least 3 characters long!")]
		public string City { get; set; }

		[Required, StringLength(64, ErrorMessage = "Street name too long!")]
		[MinLength(3, ErrorMessage = "Street name must be at least 3 characters long!")]
		public string Street { get; set; }

		[Required, Range(1, 99999, ErrorMessage = "Street Number must be within between 1 and 99999!")]
		public int StNumber { get; set; }

		[Required, Range(1, 99999, ErrorMessage = "Zip code must be within range between 1 and 99999!")]
		public int ZipCode { get; set; }

		[Required, EmailAddress]
		public string Email { get; set; }

		[Required, Phone]
		public string Phone { get; set; }

		[Required]
		public DateTime BirthDate { get; set; }
	}
}
