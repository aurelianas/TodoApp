using ToDoApp.ApplicationLayer.Services.Contracts;
using ToDoApp.Shared.Models;
using ToDoApp.Shared.Repositories;

namespace ToDoApp.ApplicationLayer.Services;

public class RoleService : IRoleService
{
	private readonly IRoleRepository _roleRepository;

	public RoleService(IRoleRepository roleRepository)
	{
		_roleRepository = roleRepository;
	}

	public async Task<List<RoleModel>> GetAll()
	{
		return await _roleRepository.GetAll();
	}
}