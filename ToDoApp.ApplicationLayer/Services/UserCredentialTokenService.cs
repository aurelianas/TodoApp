using System.Reflection;
using ToDoApp.ApplicationLayer.Services.Contracts;
using ToDoApp.Shared.Models;
using ToDoApp.Shared.Repositories;

namespace ToDoApp.ApplicationLayer.Services;

public class UserCredentialTokenService : IUserCredentialTokenService
{
	private readonly IUserCredentialTokenRepository _userCredentialTokenRepository;

	public UserCredentialTokenService(IUserCredentialTokenRepository userCredentialTokenRepository)
	{
		_userCredentialTokenRepository = userCredentialTokenRepository;
	}

	public async Task<UserCredentialTokenModel> Save(UserCredentialTokenModel model)
	{
		return await _userCredentialTokenRepository.Save(model);
	}

	public async Task<UserCredentialTokenModel?> Delete(int id)
	{
		return await _userCredentialTokenRepository.Delete(id);
	}

	public async Task<UserCredentialModel?> GetUserCredentialByToken(string refeshToken)
	{
		return await _userCredentialTokenRepository.GetUserCredentialByToken(refeshToken);
	}
}