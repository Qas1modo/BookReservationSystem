using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
	public class Genre : BaseEntity
	{
		[Required, MaxLength(32)]
		public required string Name { get; set; }
	}
}
