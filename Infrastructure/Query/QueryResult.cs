namespace Infrastructure.Query
{
	public class QueryResult<TEntity>
	{
		public int ItemsCount { get; }

		public int? PageNumber { get; }

		public int PageSize { get; }

		public bool PagingEnabled => PageNumber != null;

		public IEnumerable<TEntity> Items { get; }

		public QueryResult(IEnumerable<TEntity> items, int itemsCount, int pageSize, int? requestedPageNumber)
		{
			this.Items = items;
			this.ItemsCount = itemsCount;
			this.PageNumber = requestedPageNumber;
			this.PageSize = pageSize;
		}
	}
}
