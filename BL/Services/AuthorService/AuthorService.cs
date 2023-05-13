using DAL.Models;
using Infrastructure.UnitOfWork;

namespace BL.Services.AuthorServ
{
	public class AuthorService : IAuthorService
	{
		private readonly IUoWAuthor uow;

		public AuthorService(IUoWAuthor uow)
		{
			this.uow = uow;
		}

		public async Task<Author> GetOrAddAuthor(string authorName)
		{
			Author? newAuthor = GetAuthorByName(authorName);
			if (newAuthor != null)
			{
				return newAuthor;
			}
			uow.AuthorRepository.Insert(new() { Name = authorName });
			await uow.CommitAsync();
			return GetExistingAuthor(authorName);
		}
		public Author? GetAuthorByName(string authorName)
		{
			return uow.AuthorRepository.GetQueryable()
			.Where(x => x.Name == authorName)
			.FirstOrDefault();
		}

		public Author GetExistingAuthor(string authorName)
		{
			return uow.AuthorRepository.GetQueryable()
			.Where(x => x.Name == authorName)
			.First();
		}
	}
}
