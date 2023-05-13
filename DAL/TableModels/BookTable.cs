using Azure;
using Azure.Data.Tables;

namespace DAL.TableModels
{
	public class BookTable : ITableEntity
	{
		public string PartitionKey { get; set; }
		public string RowKey { get; set; }
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }
		public int Id { get; set; }
		public string Name { get; set; }
		public string Author { get; set; }
		public string Genre { get; set; }
		public int Total { get; set; }
		public string? Description { get; set; }
		public double Price { get; set; }
		public bool OnStock { get; set; }
	}
}
