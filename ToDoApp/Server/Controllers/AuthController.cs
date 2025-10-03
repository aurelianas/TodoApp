using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDoApp.ApplicationLayer.Services.Contracts;
using ToDoApp.Shared;
using ToDoApp.Shared.Models;

namespace ToDoApp.Server.Controllers;

[ApiController]
public class AuthController : Controller
{
	private readonly IAuthService _authService;
	private readonly IUserCredentialService _userCredentialService;
	private const string RefreshTokenKey = "refreshToken";

	public AuthController(IAuthService authService, IUserCredentialService userCredentialService)
	{
		_authService = authService;
		_userCredentialService = userCredentialService;
	}

	[HttpPost]
	[Route(ApiEndpoints.AuthEndpoints.Register)]
	public async Task<IActionResult> Register(AuthModel model)
	{
		var existUserCredential = await _userCredentialService.GetByUserName(model.UserName);

		if (existUserCredential is not null)
		{
			return BadRequest("User already exists!");
		}

		model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
		var result = await _authService.Register(model);
		return Ok(result);
	}

	[HttpPost]
	[Route(ApiEndpoints.AuthEndpoints.Login)]
	public async Task<IActionResult> Login(AuthModel model)
	{
		var existUserCredential = await _userCredentialService.GetByUserName(model.UserName);

		if (existUserCredential is null)
		{
			return BadRequest("User or password are incorrect!");
		}

		try
		{
			var valid = BCrypt.Net.BCrypt.Verify(model.Password, existUserCredential.Password);
			if (!valid)
			{
				return BadRequest("User or password are incorrect!");
			}
		}
		catch (Exception ex)
		{
			return BadRequest("User or password are incorrect!");
		}

		var result = await _authService.Login(model);
		var token = CreateAuthToken(result);
		result.RefreshToken = CreateRefreshToken(result);
		result.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(365);
		await _userCredentialService.Save(result);
		AddRefreshTokenCookie(result.RefreshToken);
		return Ok(token);
	}

	[HttpPost]
	[Route(ApiEndpoints.AuthEndpoints.Logout)]
	public async Task<IActionResult> Logout()
	{
		if (Request.Cookies.TryGetValue(RefreshTokenKey, out var refreshToken))
		{
			var userCredential = await _userCredentialService.GetByRefeshToken(refreshToken);

			if (userCredential is not null)
			{
				userCredential.RefreshToken = null;
				userCredential.RefreshTokenExpireDate = null;
				await _userCredentialService.Save(userCredential);
				Response.Cookies.Delete(RefreshTokenKey);
			}
		}

		return Ok(true);
	}

	[HttpPost]
	[Route(ApiEndpoints.AuthEndpoints.RefreshToken)]
	public async Task<IActionResult> RefreshToken()
	{
		if (!Request.Cookies.TryGetValue(RefreshTokenKey, out var refreshToken))
			return Unauthorized();

		var userCredential = await _userCredentialService.GetByRefeshToken(refreshToken);

		if (userCredential == null || userCredential.RefreshTokenExpireDate < DateTime.UtcNow)
			return Unauthorized();

		var newAuthToken = CreateAuthToken(userCredential);
		var newRefreshToken = CreateRefreshToken(userCredential);

		// update refresh token in DB
		userCredential.RefreshToken = newRefreshToken;
		userCredential.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(365);
		await _userCredentialService.Save(userCredential);
		AddRefreshTokenCookie(userCredential.RefreshToken);
		return Ok(newAuthToken);
	}

	private string CreateAuthToken(UserCredentialModel userCredential)
	{
		List<Claim> claims = new()
		{
			new Claim("userCredentialId", $"{userCredential.Id}"),
			new Claim("userName", $"{userCredential.UserName}"),
			new Claim("userProfileId", $"{userCredential.UserProfileId}")
		};

		foreach (var role in userCredential.Roles)
		{
			claims.Add(new Claim(ClaimTypes.Role, role.Name));
		}

		var secretJwtKey = WebApplication.CreateBuilder().Configuration.GetSection("Jwt")["Key"];
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretJwtKey));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

		var token = new JwtSecurityToken(
			issuer: "https://localhost:7217/",
			audience: "https://localhost:7217/",
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(2),
			signingCredentials: creds
		);

		var jwt = new JwtSecurityTokenHandler().WriteToken(token);
		return jwt;
	}

	private string CreateRefreshToken(UserCredentialModel userCredential)
	{
		List<Claim> claims = new()
		{
			new Claim("userCredentialId", $"{userCredential.Id}"),
			new Claim("userName", $"{userCredential.UserName}"),
			new Claim("userProfileId", $"{userCredential.UserProfileId}")
		};

		var secretJwtKey = WebApplication.CreateBuilder().Configuration.GetSection("Jwt")["Key"];
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretJwtKey));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

		var token = new JwtSecurityToken(
			issuer: "https://localhost:7217/",
			audience: "https://localhost:7217/",
			claims: claims,
			expires: DateTime.UtcNow.AddDays(365),
			signingCredentials: creds
		);

		var jwt = new JwtSecurityTokenHandler().WriteToken(token);
		return jwt;
	}

	private void AddRefreshTokenCookie(string refreshToken)
	{
		Response.Cookies.Append(RefreshTokenKey, refreshToken, new CookieOptions
		{
			HttpOnly = true,
			Secure = true,
			SameSite = SameSiteMode.Strict,
			Expires = DateTime.UtcNow.AddDays(365)
		});
	}
}