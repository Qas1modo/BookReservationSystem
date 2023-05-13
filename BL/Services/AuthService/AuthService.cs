using AutoMapper;
using BL.DTOs;
using BL.QueryObjects;
using DAL.Models;
using Infrastructure.UnitOfWork;
using System.Security.Cryptography;

namespace BL.Services.AuthServ
{
	public class AuthService : IAuthService
	{
		private readonly IUoWUserInfo uoWUserInfo;
		private readonly IMapper mapper;
		private readonly UserQueryObject queryObject;
		private const int saltSize = 16;
		private const int iterations = 100000;
		private const int hashLen = 32;
		public AuthService(IUoWUserInfo userInfo,
			IMapper mapper,
			UserQueryObject query)
		{
			this.uoWUserInfo = userInfo;
			this.mapper = mapper;
			queryObject = query;
		}

		public async Task RegisterUserAsync(RegistrationDto input)
		{
			using (var hashedPassword = new Rfc2898DeriveBytes(input.OpenPassword, saltSize, iterations, HashAlgorithmName.SHA256))
			{
				input.Salt = Convert.ToBase64String(hashedPassword.Salt);
				input.Password = Convert.ToBase64String(hashedPassword.GetBytes(hashLen));
			}
			input.Group = DAL.Enums.Group.User;
			User user = mapper.Map<User>(input);
			uoWUserInfo.UserRepository.Insert(user);
			await uoWUserInfo.CommitAsync();
		}

		public UserAuthDto? Login(UserLoginDto input)
		{
			User? user = queryObject.GetUserByEmail(input.Email);
			if (user == null || !VerifyPassword(user.Password, user.Salt, input.Password))
			{
				return null;
			}
			return mapper.Map<User, UserAuthDto>(user);
		}

		private static bool VerifyPassword(string storedPassword, string storedSalt, string verifyPassword)
		{
			byte[] password = Convert.FromBase64String(storedPassword);
			byte[] salt = Convert.FromBase64String(storedSalt);
			using var hashedPassword = new Rfc2898DeriveBytes(verifyPassword, salt, iterations, HashAlgorithmName.SHA256);
			var inputPassword = hashedPassword.GetBytes(hashLen);
			return inputPassword.SequenceEqual(password);
		}

		public async Task<bool> ChangePasswordAsync(ChangePasswordDto input)
		{
			User user = await uoWUserInfo.UserRepository.GetByID(input.UserId ?? -1);
			if (user == null || !VerifyPassword(user.Password, user.Salt, input.OldPassword))
			{
				return false;
			}
			using (var hashedPassword = new Rfc2898DeriveBytes(input.NewPassword, saltSize, iterations, HashAlgorithmName.SHA256))
			{
				user.Salt = Convert.ToBase64String(hashedPassword.Salt);
				user.Password = Convert.ToBase64String(hashedPassword.GetBytes(hashLen));
			}
			uoWUserInfo.UserRepository.Update(user);
			await uoWUserInfo.CommitAsync();
			return true;
		}
	}
}
