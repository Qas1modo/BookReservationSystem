using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
	public class Review : BaseEntity
	{
		public int UserId { get; set; }

		[ForeignKey(nameof(UserId))]
		public virtual User User { get; set; }

		public int BookId { get; set; }

		[ForeignKey(nameof(BookId))]
		public virtual Book Book { get; set; }

		[Range(0, 10), Required]
		public int Score { get; set; }
		public DateTime AddedAt { get; set; }
		public string? Description { get; set; }
	}
}
