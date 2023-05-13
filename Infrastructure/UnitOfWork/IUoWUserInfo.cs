using DAL.Models;
using Infrastructure.Repository;


namespace Infrastructure.UnitOfWork
{
	public interface IUoWUserInfo : IUnitOfWork
	{
		IRepository<User> UserRepository { get; }
	}
}
