using Azure;
using Azure.Data.Tables;

namespace DAL.TableModels
{
	public class GenreTable : ITableEntity
	{
		public string PartitionKey { get; set; }
		public string RowKey { get; set; }
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }
		public string Name { get; set; }

		public int Id;
	}
}
