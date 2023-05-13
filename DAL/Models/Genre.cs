using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
	public class Genre : BaseEntity
	{
		[Required, MaxLength(32)]
		public string Name { get; set; }
	}
}
