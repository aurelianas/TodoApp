using Microsoft.EntityFrameworkCore;
using ToDoApp.Shared.Models;

namespace ToDoApp.PersistenceLayer.EnityConfiguration;

public class UserCredentialTokenConfiguration
{
	public static void Configure(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<UserCredentialTokenModel>()
				.HasKey(e => e.Id);

		modelBuilder.Entity<UserCredentialTokenModel>()
			.Property(e => e.Id)
			.UseIdentityColumn(1, 1);

		modelBuilder.Entity<UserCredentialTokenModel>()
			.HasOne(e => e.UserCredential)
			.WithMany(e => e.UserCredentialTokens)
			.HasForeignKey(e => e.UserCredentialId)
			.OnDelete(DeleteBehavior.Cascade); // optional: deletes UserCredentialTokens if UserCredential is deleted
	}
}