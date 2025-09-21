using ToDoApp.Shared.Models;

namespace ToDoApp.Shared.Repositories;

public interface IAuthRepository : IBaseRepository<AuthModel>
{
	Task<UserCredentialModel> Register(AuthModel model);

	Task<UserCredentialModel> Login(AuthModel model);

	Task<UserCredentialModel> RefreshToken(int userCredentialId);
}