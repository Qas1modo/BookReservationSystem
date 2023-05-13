using BL.DTOs;

namespace BL.Services.AuthServ
{
	public interface IAuthService
	{
		Task RegisterUserAsync(RegistrationDto input);
		UserAuthDto? Login(UserLoginDto input);
		Task<bool> ChangePasswordAsync(ChangePasswordDto input);
	}
}
