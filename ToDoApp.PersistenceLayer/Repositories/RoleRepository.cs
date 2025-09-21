using ToDoApp.PersistenceLayer.Data;
using ToDoApp.Shared.Models;
using ToDoApp.Shared.Repositories;

namespace ToDoApp.PersistenceLayer.Repositories;

public class RoleRepository : BaseRepository<RoleModel>, IRoleRepository
{
	public RoleRepository(DatabaseContext databaseContext) : base(databaseContext)
	{
	}
}