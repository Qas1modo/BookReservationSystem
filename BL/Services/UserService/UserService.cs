using AutoMapper;
using BL.DTOs;
using BL.DTOs.BasicDtos;
using BL.QueryObjects;
using DAL.Enums;
using DAL.Models;
using Infrastructure.Query;
using Infrastructure.UnitOfWork;

namespace BL.Services.UserServ
{
	public class UserService : IUserService
	{

		private readonly IMapper mapper;
		private readonly IUoWUserInfo uow;
		private readonly UserQueryObject queryObject;
		private readonly IQuery<User> query;

		public UserService(IUoWUserInfo uow, IMapper mapper, UserQueryObject userQuery, IQuery<User> query)
		{
			this.mapper = mapper;
			this.uow = uow;
			queryObject = userQuery;
			this.query = query;
		}

		public async Task UpdateUserDataAsync(PersonalInfoDto input, int userId)
		{
			User user = await uow.UserRepository.GetByID(userId);
			uow.UserRepository.Update(mapper.Map(input, user));
			await uow.CommitAsync();
		}

		public async Task<PersonalInfoDto> ShowUserData(int userId)
		{
			User user = await uow.UserRepository.GetByID(userId);
			return mapper.Map<User, PersonalInfoDto>(user);
		}

		public int IdUserWithEmail(string email)
		{
			User? user = queryObject.GetUserByEmail(email);
			return user == null ? -1 : user.Id;
		}

		public int IdUserWithUsername(string username)
		{
			User? user = queryObject.GetUserByName(username);
			return user == null ? -1 : user.Id;
		}

		public async Task<QueryResultDto<UserDto>> ShowUsers(int pageNumber)
		{
			query.Page(pageNumber, 20);
			var result = await query.Execute();
			return mapper.Map<QueryResult<User>, QueryResultDto<UserDto>>(result);
		}

		public async Task UpdateUserPermission(int userId, Group newGroup)
		{
			User user = await uow.UserRepository.GetByID(userId);
			user.Group = newGroup;
			uow.UserRepository.Update(user);
			await uow.CommitAsync();
		}
	}
}
