using Microsoft.AspNetCore.Mvc;
using ToDoApp.ApplicationLayer.Services.Contracts;
using ToDoApp.Shared;
using ToDoApp.Shared.Models;

namespace ToDoApp.Server.Controllers;

[ApiController]
public class UserCredentialController : Controller
{
	private readonly IUserCredentialService _userCredentialService;

	public UserCredentialController(IUserCredentialService userCredentialService)
	{
		_userCredentialService = userCredentialService;
	}

	[HttpGet]
	[Route(ApiEndpoints.UserCredentialEndpoints.GetById)]
	public async Task<IActionResult> GetById([FromRoute] int id)
	{
		var result = await _userCredentialService.GetById(id);

		if (result != null)
		{
			return Ok(result);
		}

		return NotFound();
	}

	[HttpGet]
	[Route(ApiEndpoints.UserCredentialEndpoints.GetByUserName)]
	public async Task<IActionResult> GetByUserName([FromQuery] string userName)
	{
		var result = await _userCredentialService.GetByUserName(userName);

		if (result != null)
		{
			return Ok(result);
		}

		return NotFound();
	}

	[HttpGet]
	[Route(ApiEndpoints.UserCredentialEndpoints.GetAllWithRoles)]
	public async Task<IActionResult> GetAllWithRoles()
	{
		var result = await _userCredentialService.GetAllWithRoles();

		if (result is not null)
		{
			return Ok(result);
		}

		return NotFound();
	}

	[HttpPost]
	[Route(ApiEndpoints.UserCredentialEndpoints.UpdateRoles)]
	public async Task<IActionResult> UpdateRoles(UserCredentialModel model)
	{
		var list = new List<RoleModel>() 
		{
			new()
			{
				Id = 1,
				Name = "Admin",
			},
			new()
			{
				Id = 2,
				Name = "User",
			}
		};
		var result = await _userCredentialService.UpdateRoles(model);

		if (result is not null)
		{
			return Ok(result);
		}

		return NotFound();
	}
}