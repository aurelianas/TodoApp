using ToDoApp.Shared.Models;

namespace ToDoApp.ApplicationLayer.Services.Contracts;

public interface IRoleService
{
	Task<List<RoleModel>> GetAll();
}