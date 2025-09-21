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
		var token = CreateToken(result);
		return Ok(token);
	}

	[HttpPost]
	[Route(ApiEndpoints.AuthEndpoints.RefreshToken)]
	public async Task<IActionResult> RefreshToken([FromRoute] int userCredentialId)
	{
		var result = await _authService.RefreshToken(userCredentialId);
		var token = CreateToken(result);
		return Ok(token);
	}

	private string CreateToken(UserCredentialModel userCredential)
	{
		List<Claim> claims = new()
		{
			new Claim("userCredentialId", $"{userCredential.Id}"),
			new Claim("userName", $"{userCredential.UserName}"),
			new Claim("userProfileId", $"{userCredential.UserProfileId}"),
			//new Claim(ClaimTypes.Role, "User")
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
			expires: DateTime.UtcNow.AddMinutes(60),
			signingCredentials: creds
		);

		var jwt = new JwtSecurityTokenHandler().WriteToken(token);

		return jwt;
	}

	//private string Hash(string clientHash)
	//{
	//	// Store as BCrypt hash (automatically includes salt + work factor)
	//	return BCrypt.Net.BCrypt.HashPassword(clientHash);
	//}

	//private bool Verify(string clientHash, string storedHash)
	//{
	//	// Compare hash from client with stored BCrypt hash
	//	return BCrypt.Net.BCrypt.Verify(clientHash, storedHash);
	//}
}