using AutoMapper;
using Azure.Messaging.ServiceBus;
using BL.Facades.BookFac;
using BL.Facades.OrderFac;
using BL.Facades.UserFac;
using BL.QueryObjects;
using BL.Services.AuthorServ;
using BL.Services.AuthServ;
using BL.Services.BookServ;
using BL.Services.CartItemServ;
using BL.Services.GenreServ;
using BL.Services.PrecomputeService;
using BL.Services.ReservationServ;
using BL.Services.ReviewServ;
using BL.Services.StockServ;
using BL.Services.UserServ;
using BL.Services.WishListItemServ;
using BL.Services.WishListItemService;
using DAL;
using DAL.Models;
using Infrastructure.EFCore.Query;
using Infrastructure.EFCore.Repository;
using Infrastructure.EFCore.UnitOfWork;
using Infrastructure.Query;
using Infrastructure.Repository;
using Infrastructure.TableStorage;
using Infrastructure.UnitOfWork;

namespace WebAppMVC.Config
{
	public class DiConfig
	{
		public static void ConfigureDi(IServiceCollection services, ConfigurationManager configuration)
		{
			// Add services to the container.
			services.AddSingleton<IMapper>(new Mapper(new MapperConfiguration(MappingConfig.ConfigureMapping)));

			services.AddSingleton(new GenreTableStorage(configuration.GetConnectionString("NoSqlDb")));
			services.AddSingleton(new BookTableStorage(configuration.GetConnectionString("NoSqlDb")));
			services.AddSingleton(new CountTableStorage(configuration.GetConnectionString("NoSqlDb")));
			services.AddSingleton(new ReviewTableStorage(configuration.GetConnectionString("NoSqlDb")));
			services.AddSingleton(new ServiceBusClient(configuration.GetConnectionString("ServerBus")));
			services.AddScoped<IPrecompute, Precompute>();
			// Queries DI Setup
			services.AddTransient<IQuery<Author>, EFQuery<Author>>();
			services.AddTransient<IQuery<Book>, EFQuery<Book>>();
			services.AddTransient<IQuery<CartItem>, EFQuery<CartItem>>();
			services.AddTransient<IQuery<Genre>, EFQuery<Genre>>();
			services.AddTransient<IQuery<Reservation>, EFQuery<Reservation>>();
			services.AddTransient<IQuery<Review>, EFQuery<Review>>();
			services.AddTransient<IQuery<User>, EFQuery<User>>();
			services.AddTransient<IQuery<WishListItem>, EFQuery<WishListItem>>();


			// Context DI Setup
			services.AddScoped<BookReservationDbContext, BookReservationDbContext>();

			// Repositories DI Setup
			services.AddScoped<IRepository<Book>, EFGenericRepository<Book>>();
			services.AddScoped<IRepository<Author>, EFGenericRepository<Author>>();
			services.AddScoped<IRepository<Genre>, EFGenericRepository<Genre>>();
			services.AddScoped<IRepository<CartItem>, EFGenericRepository<CartItem>>();
			services.AddScoped<IRepository<Reservation>, EFGenericRepository<Reservation>>();
			services.AddScoped<IRepository<Review>, EFGenericRepository<Review>>();
			services.AddScoped<IRepository<User>, EFGenericRepository<User>>();
			services.AddScoped<IRepository<WishListItem>, EFGenericRepository<WishListItem>>();

			// UnitOfWork DI Setup
			services.AddScoped<IUoWBook, EFUoWBook>();
			services.AddScoped<IUoWAuthor, EFUoWAuthor>();
			services.AddScoped<IUoWCartItem, EFUoWCartItem>();
			services.AddScoped<IUoWCartItem, EFUoWCartItem>();
			services.AddScoped<IUoWGenre, EFUoWGenre>();
			services.AddScoped<IUoWReservation, EFUoWReservation>();
			services.AddScoped<IUoWReview, EFUoWReview>();
			services.AddScoped<IUoWUserInfo, EFUoWUserInfo>();
			services.AddScoped<IUoWUser, EFUoWUser>();
			services.AddScoped<IUoWWishList, EFUoWWishList>();

			// Services DI Setup
			services.AddScoped<IStockService, StockService>();
			services.AddScoped<ICartItemService, CartItemService>();
			services.AddScoped<IReservationService, ReservationService>();
			services.AddScoped<IReviewService, ReviewService>();
			services.AddScoped<IBookService, BookService>();
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IGenreService, GenreService>();
			services.AddScoped<IAuthorService, AuthorService>();
			services.AddScoped<IWishListItemService, WishListItemService>();

			// Facades and QO DI Setup
			services.AddScoped<IOrderFacade, OrderFacade>();
			services.AddScoped<UserQueryObject, UserQueryObject>();
			services.AddScoped<GenreQueryObject, GenreQueryObject>();
			services.AddScoped<IBookFacade, BookFacade>();
			services.AddScoped<IUserFacade, UserFacade>();

		}
	}
}
