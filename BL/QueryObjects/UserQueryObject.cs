using DAL.Models;
using Infrastructure.UnitOfWork;

namespace BL.QueryObjects
{
	public class UserQueryObject
	{

		private readonly IUoWUserInfo uow;

		public UserQueryObject(IUoWUserInfo uoWUserInfo)
		{
			this.uow = uoWUserInfo;
		}

		public User? GetUserByEmail(string email)
		{
			return uow.UserRepository.GetQueryable()
				.Where(x => x.Email == email)
				.FirstOrDefault();
		}

		public User? GetUserByName(string name)
		{
			return uow.UserRepository.GetQueryable()
				.Where(x => x.Name == name)
				.FirstOrDefault();
		}
	}
}
