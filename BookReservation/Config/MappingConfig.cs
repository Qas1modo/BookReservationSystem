using AutoMapper;
using BL.DTOs;
using BL.DTOs.BasicDtos;
using DAL.Models;
using DAL.TableModels;
using Infrastructure.Query;
using Infrastructure.TableStorage;
using WebAppMVC.Models;

namespace WebAppMVC.Config
{
	public class MappingConfig
	{
		public static void ConfigureMapping(IMapperConfigurationExpression config)
		{
			config.CreateMap<QueryResult<Book>, QueryResultDto<BookBasicInfoDto>>();
			config.CreateMap<RegistrationDto, User>().ReverseMap();
			config.CreateMap<BookAvailabilityDto, Book>().ReverseMap();
			config.CreateMap<CartItem, CartItemDetailDto>()
				.ForMember(dest => dest.GenreName, cfg => cfg.MapFrom(src => src.Book.Genre.Name))
				.ForMember(dest => dest.Name, cfg => cfg.MapFrom(src => src.Book.Name))
				.ForMember(dest => dest.AuthorName, cfg => cfg.MapFrom(src => src.Book.Author.Name))
				.ForMember(dest => dest.Price, cfg => cfg.MapFrom(src => src.Book.Price));
			config.CreateMap<PersonalInfoDto, User>().ReverseMap();
			config.CreateMap<AuthorDto, Author>().ReverseMap();
			config.CreateMap<BookDto, Book>()
				.ForMember(dest => dest.Deleted, cfg => cfg.MapFrom(src => false))
				.ReverseMap();
			config.CreateMap<WishListItem, WishListDetailDto>()
				.ForMember(dest => dest.GenreName, cfg => cfg.MapFrom(src => src.Book.Genre.Name))
				.ForMember(dest => dest.Name, cfg => cfg.MapFrom(src => src.Book.Name))
				.ForMember(dest => dest.AuthorName, cfg => cfg.MapFrom(src => src.Book.Author.Name))
				.ForMember(dest => dest.Price, cfg => cfg.MapFrom(src => src.Book.Price))
				.ReverseMap();
			config.CreateMap<Review, ReviewDetailDto>()
				.ForMember(dest => dest.User, cfg => cfg.MapFrom(src => src.User.Name))
				.ForMember(dest => dest.AddedAt,
				cfg => cfg.MapFrom(src => src.AddedAt.ToLocalTime().ToShortDateString()));
			config.CreateMap<CartItemDto, CartItem>().ReverseMap();
			config.CreateMap<GenreDto, Genre>().ReverseMap();
			config.CreateMap<QueryResult<Reservation>, QueryResultDto<ReservationDetailDto>>();
			config.CreateMap<QueryResult<Review>, QueryResultDto<ReviewDetailDto>>();
			config.CreateMap<QueryResult<User>, QueryResultDto<UserDto>>();
			config.CreateMap<QueryResult<WishListItem>, QueryResultDto<WishListDetailDto>>();
			config.CreateMap<Reservation, ReservationDetailDto>()
				.ForMember(dest => dest.Name, cfg => cfg.MapFrom(src => src.Book.Name))
				.ForMember(dest => dest.Author, cfg => cfg.MapFrom(src => src.Book.Author.Name))
				.ForMember(dest => dest.Genre, cfg => cfg.MapFrom(src => src.Book.Genre.Name))
				.ForMember(dest => dest.Price, cfg => cfg.MapFrom(src => src.Book.Price));
			config.CreateMap<ReservationDto, Reservation>()
				.ForMember(dest => dest.State, cfg => cfg.MapFrom(src => src.State));
			config.CreateMap<ReviewDto, Review>()
				.ForMember(dest => dest.AddedAt, cfg => cfg.MapFrom(src => src.AddedAt.ToUniversalTime()));
			config.CreateMap<UserDto, User>().ReverseMap();
			config.CreateMap<CartItem, ReservationDto>().ReverseMap();
			config.CreateMap<Book, BookBasicInfoDto>()
				.ForMember(dest => dest.Genre, cfg => cfg.MapFrom(src => src.Genre.Name))
				.ForMember(dest => dest.Author, cfg => cfg.MapFrom(src => src.Author.Name))
				.ForMember(dest => dest.Stock, cfg => cfg.MapFrom(src => src.Stock > 0 ? "Available" : "Unavailable"));
			config.CreateMap<User, UserAuthDto>().ReverseMap();
			config.CreateMap<AdminPageBookModel, BookDto>()
				.ForMember(dest => dest.Stock, cfg => cfg.MapFrom(src => src.Total))
				.ForMember(dest => dest.Deleted, cfg => cfg.MapFrom(src => false))
				.ForMember(dest => dest.Author, cfg => cfg.MapFrom(src => new Author { Name = src.Author }))
				.ForMember(dest => dest.Genre, cfg => cfg.MapFrom(src => new Genre { Name = src.Genre }))
				.ReverseMap();
			config.CreateMap<BookBasicInfoDto, AdminPageBookModel>().ReverseMap();
			config.CreateMap<Book, BookTableUpdateDto>()
				.ForMember(dest => dest.Genre, cfg => cfg.MapFrom(src => src.Genre.Name))
				.ForMember(dest => dest.Author, cfg => cfg.MapFrom(src => src.Author.Name))
				.ForMember(dest => dest.OnStock, cfg => cfg.MapFrom(src => src.Stock > 0));
			config.CreateMap<WishListItem, WishListItemDto>().ReverseMap();
			config.CreateMap<BookTable, BookBasicInfoDto>()
				.ForMember(dest => dest.Deleted, cfg => cfg.MapFrom(src => false))
				.ForMember(dest => dest.Stock, cfg => cfg.MapFrom(src => src.OnStock ? "Available" : "Unavailable"));
			config.CreateMap<Book, BookTable>()
				.ForMember(dest => dest.Author, cfg => cfg.MapFrom(src => src.Author.Name))
				.ForMember(dest => dest.Genre, cfg => cfg.MapFrom(src => src.Genre.Name))
				.ForMember(dest => dest.OnStock, cfg => cfg.MapFrom(src => src.Stock > 0));
			config.CreateMap<BookTable, BookTable>()
				.ForMember(dest => dest.Timestamp, cfg => cfg.Ignore())
				.ForMember(dest => dest.ETag, cfg => cfg.Ignore())
				.ForMember(dest => dest.PartitionKey, cfg => cfg.Ignore())
				.ForMember(dest => dest.RowKey, cfg => cfg.Ignore());
			config.CreateMap<Review, ReviewTable>()
				.ForMember(dest => dest.User, cfg => cfg.MapFrom(src => src.User.Name));
			config.CreateMap<ReviewTable, ReviewDetailDto>()
				.ForMember(dest => dest.AddedAt,
				cfg => cfg.MapFrom(src => src.AddedAt)); ;
		}
	}
}
