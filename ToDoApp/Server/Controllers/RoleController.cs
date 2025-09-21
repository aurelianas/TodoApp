using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.ApplicationLayer.Services.Contracts;
using ToDoApp.Shared;

namespace ToDoApp.Server.Controllers;

[Authorize]
[ApiController]
public class RoleController : Controller
{
	private readonly IRoleService _roleService;

	public RoleController(IRoleService roleService)
	{
		_roleService = roleService;
	}

	[Authorize(Roles = "Admin")]
	[HttpGet]
	[Route(ApiEndpoints.RoleEndpoints.GetAll)]
	public async Task<IActionResult> GetAll()
	{
		return Ok(await _roleService.GetAll());
	}
}