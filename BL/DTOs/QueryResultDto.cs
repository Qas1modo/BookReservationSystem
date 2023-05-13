namespace BL.DTOs
{
	public class QueryResultDto<TEntity>
	{
		public int ItemsCount { get; set; }

		public int? PageNumber { get; set; }

		public int PageSize { get; set; }

		public IEnumerable<TEntity> Items { get; set; }
	}
}
