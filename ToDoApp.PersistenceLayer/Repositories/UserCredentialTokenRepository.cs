using Microsoft.EntityFrameworkCore;
using ToDoApp.PersistenceLayer.Data;
using ToDoApp.Shared.Models;
using ToDoApp.Shared.Repositories;

namespace ToDoApp.PersistenceLayer.Repositories;

public class UserCredentialTokenRepository : BaseRepository<UserCredentialTokenModel>, IUserCredentialTokenRepository
{
	private readonly DatabaseContext _databaseContext;

	public UserCredentialTokenRepository(DatabaseContext databaseContext) : base(databaseContext)
	{
		_databaseContext = databaseContext;
	}

	public async Task<UserCredentialModel?> GetUserCredentialByToken(string refreshToken)
	{
		return await _databaseContext.UserCredential.Include(e => e.Roles).Include(e => e.UserCredentialTokens).FirstOrDefaultAsync(e => e.UserCredentialTokens.Any(u => u.Value == refreshToken));
	}
}