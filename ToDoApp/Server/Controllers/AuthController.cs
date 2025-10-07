using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Security.Claims;
using System.Text;
using ToDoApp.ApplicationLayer.Services.Contracts;
using ToDoApp.Shared;
using ToDoApp.Shared.Models;
using ToDoApp.Shared.Utils;

namespace ToDoApp.Server.Controllers;

[ApiController]
public class AuthController : Controller
{
	private readonly IAuthService _authService;
	private readonly IUserCredentialService _userCredentialService;
	private readonly IUserCredentialTokenService _userCredentialTokenService;
	private const string RefreshTokenKey = "refreshToken";
	private readonly IMemoryCache _memoryCache;

	public AuthController(IAuthService authService,
		IUserCredentialService userCredentialService,
		IMemoryCache memoryCache,
		IUserCredentialTokenService userCredentialTokenService)
	{
		_authService = authService;
		_userCredentialService = userCredentialService;
		_memoryCache = memoryCache;
		_userCredentialTokenService = userCredentialTokenService;
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
		var userCredential = await _userCredentialService.GetByUserName(model.UserName);

		if (userCredential is null)
		{
			return BadRequest("User or password are incorrect!");
		}

		try
		{
			var valid = BCrypt.Net.BCrypt.Verify(model.Password, userCredential.Password);
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
		var authToken = CreateAuthToken(result);
		var refreshToken = CreateRefreshToken(result);
		AddRefreshTokenCookie(refreshToken); //using cookies method and the token will be visible in the browser
		//AddRefreshTokenCookieInMemory(refreshToken); //using memory cache and the token will be not visible in the browser

		var userCredentialTokenModel = new UserCredentialTokenModel
		{
			Value = refreshToken,
			ExpirationDate = DateTime.UtcNow.AddDays(365),
			UserCredentialId = userCredential.Id
		};

		await _userCredentialTokenService.Save(userCredentialTokenModel);

		return Ok(authToken);
	}

	[HttpPost]
	[Route(ApiEndpoints.AuthEndpoints.Logout)]
	public async Task<IActionResult> Logout()
	{
		//using cookies method and the token will be visible in the browser
		if (Request.Cookies.TryGetValue(RefreshTokenKey, out var refreshToken))
		{
			var userCredential = await _userCredentialTokenService.GetUserCredentialByToken(refreshToken);

			if (userCredential is not null)
			{
				var userCredToken = userCredential.UserCredentialTokens.First(i => i.Value == refreshToken);
				await _userCredentialTokenService.Delete(userCredToken.Id);
				Response.Cookies.Delete(RefreshTokenKey);
			}
		}

		return Ok(true);

		//using memory cache and the token will be not visible in the browser
		//if (_memoryCache.TryGetValue(RefreshTokenKey, out string refreshToken))
		//{
		//	var userCredential = await _userCredentialTokenService.GetUserCredentialByToken(refreshToken);

		//	if (userCredential is not null)
		//	{
		//		var userCredToken = userCredential.UserCredentialTokens.First(i => i.Value == refreshToken);
		//		await _userCredentialTokenService.Delete(userCredToken.Id);
		//		_memoryCache.Remove(RefreshTokenKey);
		//	}
		//}

		//return Ok(true);
	}

	[HttpPost]
	[Route(ApiEndpoints.AuthEndpoints.RefreshToken)]
	public async Task<IActionResult> RefreshToken()
	{
		//using cookies method and the token will be visible in the browser
		if (!Request.Cookies.TryGetValue(RefreshTokenKey, out var refreshToken))
			return Unauthorized();

		//using memory cache and the token will be not visible in the browser
		//if (!_memoryCache.TryGetValue(RefreshTokenKey, out string refreshToken))
		//	return Unauthorized();

		var userCredential = await _userCredentialTokenService.GetUserCredentialByToken(refreshToken);

		if (userCredential is null || userCredential?.UserCredentialTokens?.FirstOrDefault()?.ExpirationDate < DateTime.UtcNow)
			return Unauthorized();

		var newAuthToken = CreateAuthToken(userCredential);
		var newRefreshToken = CreateRefreshToken(userCredential);

		// update refresh token in DB
		var userCredToken = userCredential.UserCredentialTokens.First(i => i.Value == refreshToken);
		userCredToken.Value = newRefreshToken;
		userCredToken.ExpirationDate = DateTime.UtcNow.AddDays(365);
		await _userCredentialTokenService.Save(userCredToken);
		AddRefreshTokenCookie(newRefreshToken); //using cookies method and the token will be visible in the browser
		//AddRefreshTokenCookieInMemory(newRefreshToken);
		return Ok(newAuthToken);
	}

	private string CreateAuthToken(UserCredentialModel userCredential)
	{
		List<Claim> claims = new()
		{
			new Claim("userCredentialId", $"{userCredential.Id}"),
			new Claim("userName", $"{userCredential.UserName}"),
			new Claim("userProfileId", $"{userCredential.UserProfileId}")
			//new Claim("refreshTokenId", $"{((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds()}") //for getting from memory cache
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
			expires: DateTime.UtcNow.AddMinutes(1),
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

	private void AddRefreshTokenCookieInMemory(string refreshToken)
	{
		_memoryCache.Set(RefreshTokenKey, refreshToken, TimeSpan.FromDays(2));
	}
}