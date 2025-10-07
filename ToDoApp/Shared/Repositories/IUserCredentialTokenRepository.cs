using ToDoApp.Shared.Models;

namespace ToDoApp.Shared.Repositories;

public interface IUserCredentialTokenRepository : IBaseRepository<UserCredentialTokenModel>
{
	Task<UserCredentialModel?> GetUserCredentialByToken(string refreshToken);
}