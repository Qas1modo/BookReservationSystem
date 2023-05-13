namespace BL.DTOs.BasicDtos
{
	public class ReviewDto
	{
		public int UserId { get; set; }

		public int BookId { get; set; }

		public int Score { get; set; }

		public DateTime AddedAt { get; set; }

		public string? Description { get; set; }
	}
}
