using DAL.Enums;

namespace BL.DTOs.BasicDtos
{
	public class ReservationDto
	{
		public int UserId { get; set; }

		public int BookId { get; set; }

		public DateTime ReservedAt { get; set; }

		public DateTime? RentedAt { get; set; }

		public DateTime? ReturnedAt { get; set; }

		public DateTime? CanceledAt { get; set; }

		public int LoanPeriod { get; set; }

		public RentState State { get; set; }
	}
}
