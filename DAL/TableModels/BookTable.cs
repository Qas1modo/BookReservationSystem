using Azure;
using Azure.Data.Tables;

namespace DAL.TableModels
{
	public class BookTable : ITableEntity
	{
		public required string PartitionKey { get; set; }
		public required string RowKey { get; set; }
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }
		public int Id { get; set; }
		public required string Name { get; set; }
		public required string Author { get; set; }
		public required string Genre { get; set; }
		public int Total { get; set; }
		public required string Description { get; set; }
		public double Price { get; set; }
		public bool OnStock { get; set; }
	}
}
