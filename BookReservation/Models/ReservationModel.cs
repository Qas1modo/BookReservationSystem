using DAL.Enums;

namespace WebAppMVC.Models
{
	public class ReservationModel<TEntity>
	{
		public int PageNumber { get; set; }
		public int PageCount { get; set; }
		public RentState CurrentState { get; set; }
		public string Group { get; set; }
		public int UserId { get; set; }
		public IEnumerable<TEntity> Items { get; set; }
	}
}
