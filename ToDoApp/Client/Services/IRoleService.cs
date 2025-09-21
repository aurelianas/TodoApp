using Refit;
using ToDoApp.Shared;
using ToDoApp.Shared.Models;

namespace ToDoApp.Client.Services;

public interface IRoleService
{
	[Get(ApiEndpoints.RoleEndpoints.GetAll)]
	Task<IApiResponse<IList<RoleModel>>> GetAll();
}