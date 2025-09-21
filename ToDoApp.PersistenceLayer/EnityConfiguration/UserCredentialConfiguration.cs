using Microsoft.EntityFrameworkCore;
using ToDoApp.Shared.Models;

namespace ToDoApp.PersistenceLayer.EnityConfiguration;

public static class UserCredentialConfiguration
{
	public static void Configure(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<UserCredentialModel>()
				.HasKey(e => e.Id);

		modelBuilder.Entity<UserCredentialModel>()
			.Property(e => e.Id)
			.UseIdentityColumn(1, 1);

		modelBuilder.Entity<UserCredentialModel>()
			.HasOne(e => e.UserProfile)
			.WithMany()
			.HasForeignKey(e => e.UserProfileId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<UserCredentialModel>()
			.HasIndex(e => e.UserProfileId)
			.IsUnique();

		//modelBuilder.Entity<UserCredentialModel>()
		//.HasMany(u => u.Roles)
		//.WithMany(r => r.UserCredentials)
		//.UsingEntity(j => j.ToTable("UserCredentialRoles")); //custom table name for the table that will contain the relation between Role and UserCredential tables

		modelBuilder.Entity<UserCredentialModel>()
			.HasMany(u => u.Roles)
			.WithMany(r => r.UserCredentials)
			.UsingEntity<Dictionary<string, object>>( // no need for a class unless extra fields
			   "UserCredentialRole", // table name
			   j => j.HasOne<RoleModel>()
					 .WithMany()
					 .HasForeignKey("RoleId")            //  manual FK name
					 .HasConstraintName("FK_UserCredentialRole_Role_RoleId") // custom FK name
					 .OnDelete(DeleteBehavior.Cascade),  // optional delete rule
			   j => j.HasOne<UserCredentialModel>()
					 .WithMany()
					 .HasForeignKey("UserCredentialId")  // manual FK name
					 .HasConstraintName("FK_UserCredentialRole_UserCredential_UserCredentialId")
					 .OnDelete(DeleteBehavior.Cascade),
			   j =>
			   {
				   j.HasKey("RoleId", "UserCredentialId"); // composite PK
				   j.ToTable("UserCredentialRole");       // table name
			   });
	}
}