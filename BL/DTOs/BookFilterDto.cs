namespace BL.DTOs
{
	public class BookFilterDto
	{
		public string? NameFilter { get; set; }

		public string? AuthorFilter { get; set; }

		public string? GenreFilter { get; set; }

		public bool OnStock { get; set; }

		public string? OrderBy { get; set; }

		public bool? Descending { get; set; }

		public int Page { get; set; }

		public int? PageSize { get; set; }
	}
}
