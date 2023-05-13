using Azure;
using Azure.Data.Tables;

namespace DAL.TableModels
{
	public class ReviewTable : ITableEntity
	{
		public string PartitionKey { get; set; }
		public string RowKey { get; set; }
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }
		public int Id { get; set; }
		public int BookId { get; set; }
		public string User { get; set; }
		public int Score { get; set; }
		public DateTime AddedAt { get; set; }
		public string? Description { get; set; }

	}
}
