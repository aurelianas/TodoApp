using Microsoft.EntityFrameworkCore;
using ToDoApp.Shared.Models;

namespace ToDoApp.PersistenceLayer.EnityConfiguration;

public static class RoleConfiguration
{
	public static void Configure(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<RoleModel>()
				.HasKey(e => e.Id);

		modelBuilder.Entity<RoleModel>()
			.Property(e => e.Id)
			.UseIdentityColumn(1, 1);
	}
}