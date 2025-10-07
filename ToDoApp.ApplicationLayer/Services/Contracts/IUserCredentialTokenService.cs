using ToDoApp.Shared.Models;

namespace ToDoApp.ApplicationLayer.Services.Contracts;

public interface IUserCredentialTokenService
{
	Task<UserCredentialTokenModel> Save(UserCredentialTokenModel model);

	Task<UserCredentialTokenModel?> Delete(int id);

	Task<UserCredentialModel?> GetUserCredentialByToken(string refeshToken);
}