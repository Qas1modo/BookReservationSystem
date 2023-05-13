namespace DAL.Constants
{
	public static class Constants
	{
		public static readonly int MAX_RESERVATIONS_USER = 6;

		public static readonly int PAGE_SIZE = 15;

		public static readonly string GENRE_TABLE = "Genres";

		public static readonly string BOOK_TABLE = "Books";

		public static readonly string COUNT_TABLE = "Counts";

		public static readonly string REVIEW_TABLE = "Reviews";

		public static readonly string COUNT_PARTITION = "Counts";

		public static readonly string GENRE_PARTITION = "Genres";

		public static readonly string REVIEW_FORMAT_STRING = "000000000000000000000000000000000";

		public static readonly string BOOK_FORMAT_STRING = "0000000000000000000000000000";

		public static readonly string PAGE_FORMAT = "00000";

		public static readonly string PRICE_FORMAT = "000000000.00";

		public static readonly string ALL_BOOKS_PARTITION = "AllBooks";

		public static readonly string COUNT_REVIEW_PARTITION = "ReviewCounts";

		public static readonly string STOCK_DEFAULT_PARTITION = "OnStockDefault";

		public static readonly string STOCK_NAME_ASC_PARTITION = "OnStockNameAsc";

		public static readonly string STOCK_NAME_DESC_PARTITION = "OnStockNameDesc";

		public static readonly string STOCK_PRICE_ASC_PARTITION = "OnStockPriceAsc";

		public static readonly string STOCK_PRICE_DESC_PARTITION = "OnStockPriceDesc";

		public static readonly string NOSTOCK_DEFAULT_PARTITION = "NoStockDefault";

		public static readonly string NOSTOCK_NAME_ASC_PARTITION = "NoStockNameAsc";

		public static readonly string NOSTOCK_NAME_DESC_PARTITION = "NoStockNameDesc";

		public static readonly string NOSTOCK_PRICE_ASC_PARTITION = "NoStockPriceAsc";

		public static readonly string NOSTOCK_PRICE_DESC_PARTITION = "NoStockPriceDesc";
	}
}
