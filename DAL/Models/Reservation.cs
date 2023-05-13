using DAL.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
	public class Reservation : BaseEntity
	{
		public int UserId { get; set; }

		[ForeignKey(nameof(UserId))]
		public virtual User User { get; set; }

		public int BookId { get; set; }

		[ForeignKey(nameof(BookId))]
		public virtual Book Book { get; set; }

		[Required]
		public DateTime ReservedAt { get; set; }

		public DateTime? CanceledAt { get; set; }

		public DateTime? RentedAt { get; set; }

		public DateTime? ReturnedAt { get; set; }

		[Range(1, 365), Required]
		public int LoanPeriod { get; set; }

		[Required]
		public RentState State { get; set; }
	}
}
