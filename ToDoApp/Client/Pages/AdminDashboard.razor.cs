using Microsoft.AspNetCore.Components;
using System.Data;
using ToDoApp.Client.Services;
using ToDoApp.Shared.Models;

namespace ToDoApp.Client.Pages;

[Route($"{PageRoute.AdminDashboard}")]
public partial class AdminDashboard
{
	[Inject] private IRoleService RoleService { get; set; }

	[Inject] private IUserCredentialService UserCredentialService { get; set; }

	private IList<RoleModel> UsersRoles { get; set; } = new List<RoleModel>();

	private IList<UserCredentialModel> UserCredentials { get; set; } = new List<UserCredentialModel>();

	private UserCredentialModel SelectedUserCredential { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await LoadData();
	}

	private async Task LoadData()
	{
		MainLayout?.ShowPageLoader();
		var result = await RoleService.GetAll();
		UsersRoles = result.Content ?? new List<RoleModel>(); ;

		var userCredentialsResult = await UserCredentialService.GetAllWithRoles();
		UserCredentials = userCredentialsResult.Content ?? new List<UserCredentialModel>();
		MainLayout?.HidePageLoader();
	}

	private void OnSelectedItem(UserCredentialModel item)
	{
		SelectedUserCredential = item;
	}

	private async Task UpdateUserRoles()
	{
		var result = await UserCredentialService.UpdateRoles(SelectedUserCredential);
		if (result.IsSuccessStatusCode)
		{
			await LoadData();
		}
	}

	private void SelectUnSelectItem(UserCredentialModel item, RoleModel role, object checkedValue)
	{
		var isSelected = (bool)checkedValue;

		if (isSelected) 
		{
			if (!item.Roles.Select(r => r.Id).ToList().Contains(role.Id))
			{
				item.Roles.Add(role);
			}
		}
		else
		{
			var selectedRole = item.Roles.FirstOrDefault(r => r.Id == role.Id);

			if (selectedRole is not null)
			{
				item.Roles.Remove(selectedRole);
			}
		}
	}
}