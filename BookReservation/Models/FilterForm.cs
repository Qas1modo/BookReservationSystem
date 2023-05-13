namespace WebAppMVC.Models
{
	public class FilterForm
	{
		public string? Genre { get; set; }
		public string? Author { get; set; }
		public string? BookName { get; set; }
		public bool Stock { get; set; }
		public string? OrderColumn { get; set; }
		public bool SortDescending { get; set; }
	}
}
