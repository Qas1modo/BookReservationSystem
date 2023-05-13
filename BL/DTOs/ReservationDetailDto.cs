using DAL.Enums;

namespace BL.DTOs
{
	public class ReservationDetailDto
	{
		public int Id { get; set; }
		public int BookId { get; set; }
		public string Name { get; set; }

		public string Author { get; set; }

		public string Genre { get; set; }

		public DateTime ReservedAt { get; set; }

		public DateTime? RentedAt { get; set; }

		public DateTime? ReturnedAt { get; set; }

		public DateTime? CanceledAt { get; set; }

		public int LoanPeriod { get; set; }

		public decimal Price { get; set; }

		public RentState State { get; set; }
	}
}
