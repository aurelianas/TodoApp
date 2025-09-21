using Refit;
using ToDoApp.Shared;
using ToDoApp.Shared.Models;

namespace ToDoApp.Client.Services;

public interface IUserCredentialService
{
	[Get(ApiEndpoints.UserCredentialEndpoints.GetById)]
	Task<UserCredentialModel?> GetById(int id);

	[Get(ApiEndpoints.UserCredentialEndpoints.GetByUserName)]
	Task<UserCredentialModel?> GetByUserName(string userName);

	[Get(ApiEndpoints.UserCredentialEndpoints.GetAllWithRoles)]
	Task<IApiResponse<IList<UserCredentialModel>>> GetAllWithRoles();

	[Post(ApiEndpoints.UserCredentialEndpoints.UpdateRoles)]
	Task<IApiResponse<UserCredentialModel>> UpdateRoles(UserCredentialModel model);
}