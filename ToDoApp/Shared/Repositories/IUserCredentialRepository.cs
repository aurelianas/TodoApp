using ToDoApp.Shared.Models;

namespace ToDoApp.Shared.Repositories;

public interface IUserCredentialRepository : IBaseRepository<UserCredentialModel>
{
	Task<UserCredentialModel?> GetByUserName(string userName);

	Task<UserCredentialModel> Register(UserCredentialModel userCredential);

	Task<UserCredentialModel> Login(UserCredentialModel userCredential);

	Task<IList<UserCredentialModel>> GetAllWithRoles();

	Task<UserCredentialModel> UpdateRoles(UserCredentialModel model);
}