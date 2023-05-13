using DAL.Enums;

namespace BL.DTOs
{
	public class UserAuthDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public Group Group { get; set; }
	}
}
