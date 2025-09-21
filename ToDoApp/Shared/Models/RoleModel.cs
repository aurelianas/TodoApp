using System.Text.Json.Serialization;

namespace ToDoApp.Shared.Models;

public class RoleModel
{
	public int Id { get; set; }

	public string Name { get; set; }

	// Back navigation if you need it
	//[JsonIgnore]
	public ICollection<UserCredentialModel> UserCredentials { get; set; } = new List<UserCredentialModel>();
}