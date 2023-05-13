using BL.DTOs;
using BL.Services.AuthServ;
using BL.Services.UserServ;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAppMVC.Controllers
{
	public class AuthController : Controller
	{

		private readonly IAuthService _authService;
		private readonly IUserService _userService;

		public AuthController(IAuthService authService,
			IUserService userService)
		{
			_authService = authService;
			_userService = userService;
		}

		[HttpGet]
		public IActionResult Register()
		{
			if (User.Identity?.IsAuthenticated ?? false)
			{
				return RedirectToAction("Index", "User");
			}
			return View("Register");
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> RegisterAsync(RegistrationDto user)
		{
			if (user.BirthDate > DateTime.Now.AddYears(-3))
			{
				ModelState.AddModelError("BirthDate", "Birthday must be more than three years before today!");
			}
			if (_userService.IdUserWithEmail(user.Email) != -1)
			{
				ModelState.AddModelError("Email", "Email already taken!");
			}
			if (_userService.IdUserWithUsername(user.Name) != -1)
			{
				ModelState.AddModelError("Name", "Username already taken!");
			}
			if (!ModelState.IsValid)
			{
				return View("Register");
			}
			await _authService.RegisterUserAsync(user);
			return RedirectToAction("Login", "Auth");
		}

		[HttpGet]
		public IActionResult Login()
		{
			if (User.Identity?.IsAuthenticated ?? false)
			{
				return RedirectToAction("Index", "User");
			}
			return View();
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> LoginAsync(UserLoginDto userLogin)
		{
			if (!ModelState.IsValid)
			{
				return View("Login");
			}
			UserAuthDto? userAuth = _authService.Login(userLogin);
			if (userAuth == null)
			{
				ModelState.AddModelError("Password", "Invalid credentials!");
				return View("Login");
			}
			await CreateTokenAsync(userAuth);
			return RedirectToAction("Index", "User");
		}

		private async Task CreateTokenAsync(UserAuthDto user)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Id.ToString()),
				new Claim(ClaimTypes.GivenName, user.Name),
				new Claim(ClaimTypes.Role, user.Group.ToString())
			};
			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			await HttpContext.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme,
				new ClaimsPrincipal(identity));
		}

		[HttpGet]
		public IActionResult Logout()
		{
			HttpContext.SignOutAsync("Cookies");
			return RedirectToAction("Index", "MainPage");
		}

		[HttpGet]
		public IActionResult ChangePassword()
		{
			if (!User.Identity?.IsAuthenticated ?? true)
			{
				return RedirectToAction("Login", "Auth");
			}
			return View("ChangePassword");

		}

		[Authorize, HttpPost]
		public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDto input)
		{
			if (!int.TryParse(User.Identity?.Name, out int userId))
			{
				ModelState.AddModelError("UserId", "Identity error!");
			}
			if (!ModelState.IsValid)
			{
				return View("ChangePassword");
			}
			input.UserId = userId;
			if (!await _authService.ChangePasswordAsync(input))
			{
				ModelState.AddModelError("Password", "Old password is invalid!");
			}
			ModelState.AddModelError("NewPassword", "Password changed sucessfully!");
			return View("ChangePassword");
		}

		public IActionResult Index()
		{
			return View();
		}
	}
}
