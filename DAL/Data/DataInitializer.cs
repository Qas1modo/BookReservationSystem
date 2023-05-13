using DAL.Enums;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
	public static class DataInitializer
	{
		public static void Seed(this ModelBuilder modelBuilder)
		{
			User user = new()
			{
				Id = 1,
				City = "Breclav",
				Street = "Hlavni",
				StNumber = 15,
				ZipCode = 03605,
				Name = "admin",
				Email = "admin@mail.com",
				Password = "79znGdbjJimGOZQdZVxcYNDAd+mJZjqP76afOyQXGuY=", //Abc123456
				Salt = "tY+cZjt0T9wFwkkBE46bkQ==",
				Phone = "911999111",
				BirthDate = new DateTime(1970, 3, 1),
				Group = Group.Admin,
			};

			User user2 = new()
			{
				Id = 2,
				City = "Brno",
				Street = "Manesova",
				StNumber = 13,
				ZipCode = 03601,
				Name = "Peter Marcin",
				Email = "peter@mail.com",
				Password = "79znGdbjJimGOZQdZVxcYNDAd+mJZjqP76afOyQXGuY=", //Abc123456
				Salt = "tY+cZjt0T9wFwkkBE46bkQ==",
				Phone = "+421911999222",
				BirthDate = new DateTime(1980, 3, 1),
				Group = Group.Employee,
			};

			User user3 = new()
			{
				Id = 3,
				City = "Praha",
				Street = "Prazska",
				StNumber = 9,
				ZipCode = 03602,
				Name = "Ferko Turan",
				Email = "feroslav1@mail.com",
				Password = "79znGdbjJimGOZQdZVxcYNDAd+mJZjqP76afOyQXGuY=", //Abc123456
				Salt = "tY+cZjt0T9wFwkkBE46bkQ==",
				Phone = "+421911999333",
				BirthDate = new DateTime(1983, 3, 1),
				Group = Group.User,
			};

			modelBuilder.Entity<User>().HasData(user);
			modelBuilder.Entity<User>().HasData(user2);
			modelBuilder.Entity<User>().HasData(user3);
			///////////////////////////////////////////////////


			///////////////////////////////////////////////////
			// ENTITY => Author Initialization

			Author author1 = new()
			{
				Id = 1,
				Name = "Joanne Rowling",
			};

			Author author2 = new()
			{
				Id = 2,
				Name = "George Martin",
			};

			Author author3 = new()
			{
				Id = 3,
				Name = "Robert Merle",
			};

			Author author4 = new()
			{
				Id = 4,
				Name = "Andrzej Sapkowski",
			};

			Author author5 = new()
			{
				Id = 5,
				Name = "Karel Hynek Mácha",
			};

			Author author6 = new()
			{
				Id = 6,
				Name = "Andrzej Sapkowski",
			};

			modelBuilder.Entity<Author>().HasData(author1);
			modelBuilder.Entity<Author>().HasData(author2);
			modelBuilder.Entity<Author>().HasData(author3);
			modelBuilder.Entity<Author>().HasData(author4);
			modelBuilder.Entity<Author>().HasData(author5);
			modelBuilder.Entity<Author>().HasData(author6);
			///////////////////////////////////////////////////


			///////////////////////////////////////////////////
			// ENTITY => Genre Initialization

			Genre genre1 = new()
			{
				Id = 1,
				Name = "Action",
			};

			Genre genre2 = new()
			{
				Id = 2,
				Name = "Horror",
			};

			Genre genre3 = new()
			{
				Id = 3,
				Name = "Thriller",
			};

			Genre genre4 = new()
			{
				Id = 4,
				Name = "Comedy",
			};

			Genre genre5 = new()
			{
				Id = 5,
				Name = "Detective",
			};

			Genre genre6 = new()
			{
				Id = 6,
				Name = "Fantasy",
			};

			Genre genre7 = new()
			{
				Id = 7,
				Name = "SciFi",
			};

			Genre genre8 = new()
			{
				Id = 8,
				Name = "Romance",
			};

			Genre genre9 = new()
			{
				Id = 9,
				Name = "Western",
			};

			Genre genre10 = new()
			{
				Id = 10,
				Name = "Dystopian",
			};

			Genre genre11 = new()
			{
				Id = 11,
				Name = "Contemporary",
			};
			modelBuilder.Entity<Genre>().HasData(genre1);
			modelBuilder.Entity<Genre>().HasData(genre2);
			modelBuilder.Entity<Genre>().HasData(genre3);
			modelBuilder.Entity<Genre>().HasData(genre4);
			modelBuilder.Entity<Genre>().HasData(genre5);
			modelBuilder.Entity<Genre>().HasData(genre6);
			modelBuilder.Entity<Genre>().HasData(genre7);
			modelBuilder.Entity<Genre>().HasData(genre8);
			modelBuilder.Entity<Genre>().HasData(genre9);
			modelBuilder.Entity<Genre>().HasData(genre10);
			modelBuilder.Entity<Genre>().HasData(genre11);
			///////////////////////////////////////////////////


			///////////////////////////////////////////////////
			// ENTITY => Book, Data Initializaton

			Book harryPotter1 = new()
			{
				Id = 1,
				Name = "Harry Potter and the Philosopher's Stone",
				AuthorId = 1,
				GenreId = 6,
				Stock = 63,
				Total = 100,
				Price = 16,
				Description = "Lorem Ipsum",
			};

			Book harryPotter2 = new()
			{
				Id = 2,
				Name = "Harry Potter and the Chamber of Secrets",
				AuthorId = 1,
				Stock = 0,
				GenreId = 7,
				Total = 100,
				Price = 30,
				Description = "Lorem Ipsum",
			};

			Book harryPotter3 = new()
			{
				Id = 3,
				Name = "Harry Potter and the Prisoner of Azkaban",
				AuthorId = 1,
				GenreId = 6,
				Stock = 900,
				Total = 1000,
				Price = 17,
				Description = "Lorem Ipsum",
			};

			Book harryPotter4 = new()
			{
				Id = 4,
				Name = "Harry Potter and the Goblet of Fire",
				AuthorId = 1,
				GenreId = 7,
				Stock = 150,
				Total = 200,
				Price = 27,
				Description = "Lorem Ipsum",
			};
			Book harryPotter5 = new()
			{
				Id = 5,
				Name = "Harry Potter and the Order of the Phoenix",
				AuthorId = 1,
				GenreId = 6,
				Stock = 7,
				Total = 50,
				Price = 50,
				Description = "Lorem Ipsum",
			};

			Book harryPotter6 = new()
			{
				Id = 6,
				Name = "Harry Potter and the Half-Blood Prince",
				AuthorId = 1,
				GenreId = 8,
				Stock = 15,
				Total = 40,
				Price = 30,
				Description = "Lorem Ipsum",
			};

			Book gameOfThrones1 = new()
			{
				Id = 7,
				Name = "Fire & Blood",
				AuthorId = 2,
				GenreId = 6,
				Stock = 90,
				Total = 100,
				Price = 23,
				Description = "Lorem Ipsum",
			};

			Book gameOfThrones2 = new()
			{
				Id = 8,
				Name = "A Game of Thrones",
				AuthorId = 2,
				GenreId = 6,
				Stock = 79,
				Total = 100,
				Price = 35,
				Description = "Lorem Ipsum",
			};

			Book gameOfThrones3 = new()
			{
				Id = 9,
				Name = "A Clash of Kings ",
				AuthorId = 2,
				GenreId = 6,
				Stock = 1,
				Total = 100,
				Price = 46,
				Description = "Lorem Ipsum",
			};

			Book gameOfThrones4 = new()
			{
				Id = 10,
				Name = "A Storm of Swords ",
				AuthorId = 2,
				GenreId = 6,
				Stock = 50,
				Total = 100,
				Price = 25,
				Description = "Lorem Ipsum",
			};

			Book gameOfThrones5 = new()
			{
				Id = 11,
				Name = "A Feast for Crows",
				AuthorId = 2,
				GenreId = 6,
				Stock = 0,
				Total = 100,
				Price = 9000,
				Description = "Lorem Ipsum",
			};

			Book witcher1 = new()
			{
				Id = 12,
				Name = "Witcher - Last Wish",
				AuthorId = 6,
				GenreId = 6,
				Stock = 100,
				Total = 100,
				Price = 25,
				Description = "Lorem Ipsum",
			};

			Book witcher2 = new()
			{
				Id = 13,
				Name = "Witcher - Sword of Destiny",
				AuthorId = 6,
				GenreId = 6,
				Stock = 100,
				Total = 100,
				Price = 30,
				Description = "Lorem Ipsum",
			};

			Book witcher3 = new()
			{
				Id = 14,
				Name = "Witcher - Blood of Elves",
				AuthorId = 6,
				GenreId = 6,
				Stock = 100,
				Total = 100,
				Price = 25,
				Description = "Lorem Ipsum",
			};

			Book witcher4 = new()
			{
				Id = 15,
				Name = "Witcher - Time of Contempt",
				AuthorId = 6,
				GenreId = 6,
				Stock = 100,
				Total = 100,
				Price = 25,
				Description = "Lorem Ipsum",
			};

			Book witcher5 = new()
			{
				Id = 16,
				Name = "Witcher - Baptism of Fire",
				AuthorId = 6,
				GenreId = 6,
				Stock = 100,
				Total = 100,
				Price = 30,
				Description = "Lorem Ipsum",
			};

			Book witcher6 = new()
			{
				Id = 17,
				Name = "Witcher - The Tower of Swallows",
				AuthorId = 6,
				GenreId = 6,
				Stock = 100,
				Total = 100,
				Price = 35,
				Description = "Lorem Ipsum",
			};

			Book witcher7 = new()
			{
				Id = 18,
				Name = "Witcher - Season of Storms",
				AuthorId = 6,
				GenreId = 6,
				Stock = 100,
				Total = 100,
				Price = 35,
				Description = "Lorem Ipsum",
			};

			Book witcher8 = new()
			{
				Id = 19,
				Name = "Witcher - Explodes",
				AuthorId = 6,
				GenreId = 6,
				Stock = 1,
				Total = 100,
				Price = 25,
				Description = "Lorem Ipsum witcherino",
			};

			modelBuilder.Entity<Book>().HasData(harryPotter1);
			modelBuilder.Entity<Book>().HasData(harryPotter2);
			modelBuilder.Entity<Book>().HasData(harryPotter3);
			modelBuilder.Entity<Book>().HasData(harryPotter4);
			modelBuilder.Entity<Book>().HasData(harryPotter5);
			modelBuilder.Entity<Book>().HasData(harryPotter6);
			modelBuilder.Entity<Book>().HasData(gameOfThrones1);
			modelBuilder.Entity<Book>().HasData(gameOfThrones2);
			modelBuilder.Entity<Book>().HasData(gameOfThrones3);
			modelBuilder.Entity<Book>().HasData(gameOfThrones4);
			modelBuilder.Entity<Book>().HasData(gameOfThrones5);
			modelBuilder.Entity<Book>().HasData(witcher1);
			modelBuilder.Entity<Book>().HasData(witcher2);
			modelBuilder.Entity<Book>().HasData(witcher3);
			modelBuilder.Entity<Book>().HasData(witcher4);
			modelBuilder.Entity<Book>().HasData(witcher5);
			modelBuilder.Entity<Book>().HasData(witcher6);
			modelBuilder.Entity<Book>().HasData(witcher7);
			modelBuilder.Entity<Book>().HasData(witcher8);
		}
	}
}
