using Azure;
using Azure.Data.Tables;

namespace DAL.TableModels
{
	public class CountTable : ITableEntity
	{
		public required string PartitionKey { get; set; }
		public required string RowKey { get; set; }
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }
		public int TotalCount { get; set; }
	}
}
