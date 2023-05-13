using DAL.Models;

namespace BL.Services.AuthorServ
{
	public interface IAuthorService
	{
		Task<Author> GetOrAddAuthor(string authorName);

		Author? GetAuthorByName(string authorName);
		Author GetExistingAuthor(string authorName);
	}
}

