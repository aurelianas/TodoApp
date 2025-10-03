using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using ToDoApp.PersistenceLayer.Data;
using ToDoApp.Shared.Models;
using ToDoApp.Shared.Repositories;

namespace ToDoApp.PersistenceLayer.Repositories;

public class UserCredentialRepository : BaseRepository<UserCredentialModel>, IUserCredentialRepository
{
	private readonly DatabaseContext _databaseContext;

	public UserCredentialRepository(DatabaseContext databaseContext) : base(databaseContext)
	{
		_databaseContext = databaseContext;
	}
	public async Task<UserCredentialModel?> GetByUserName(string userName)
	{
		var userCredential = await _databaseContext.UserCredential.FirstOrDefaultAsync(u => u.UserName == userName);

		if (userCredential is not null)
		{
			return userCredential;
		}

		return null;
	}

	public async Task<UserCredentialModel> Register(UserCredentialModel userCredential)
	{
		await _databaseContext.UserProfile.AddAsync(userCredential.UserProfile);
		await _databaseContext.UserCredential.AddAsync(userCredential);
		await _databaseContext.SaveChangesAsync();

		return userCredential;
	}

	public async Task<UserCredentialModel> Login(UserCredentialModel userCredential)
	{
		var userCred = await _databaseContext.UserCredential.FirstOrDefaultAsync(u => u.UserName == userCredential.UserName);
		return userCred ?? new();
	}

	public async Task<IList<UserCredentialModel>> GetAllWithRoles()
	{
		var result = await _databaseContext.UserCredential.Include(e => e.Roles).Include(e => e.UserProfile).ToListAsync();
		return result ?? new();
	}

	public async Task<UserCredentialModel> UpdateRoles(UserCredentialModel model)
	{
		var userCredential = await _databaseContext.UserCredential.Include(e => e.Roles).FirstOrDefaultAsync(e => e.Id == model.Id);
		var roleIds = model.Roles.Select(r => r.Id).ToList();

		userCredential?.Roles.Clear();

		// Attach roles by their Ids (so EF doesn't try to insert duplicates)
		foreach (var role in model.Roles)
		{
			var existingRole = await _databaseContext.Role.FindAsync(role.Id);
			if (existingRole != null)
			{
				userCredential?.Roles.Add(existingRole);
			}
		}

		await _databaseContext.SaveChangesAsync();
		return userCredential ?? new();
	}

	public async Task<UserCredentialModel> GetByRefreshToken(string refreshToken)
	{
		return await _databaseContext.UserCredential.Include(e => e.Roles).FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
	}

	public async Task<bool> DeleteRefreshToken(string refreshToken)
	{
		var userCredential =  await _databaseContext.UserCredential.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

		if (userCredential is not null)
		{
			userCredential.RefreshToken = null;
			userCredential.RefreshTokenExpireDate = null;
			_databaseContext.UserCredential.Update(userCredential);
			await _databaseContext.SaveChangesAsync();
			return true;
		}

		return false;
	}
}