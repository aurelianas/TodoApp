namespace ToDoApp.Shared.Models;

public class UserCredentialTokenModel
{
	public int Id { get; set; }

	public string Value { get; set; }

	public DateTime ExpirationDate { get; set; }

	public int UserCredentialId { get; set; }
	
	public UserCredentialModel UserCredential { get; set; }
}