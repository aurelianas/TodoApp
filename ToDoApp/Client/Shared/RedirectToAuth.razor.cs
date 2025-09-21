using Microsoft.AspNetCore.Components;

namespace ToDoApp.Client.Shared;

public partial class RedirectToAuth
{
	[Inject] private NavigationManager? NavigationManager { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		NavigationManager!.NavigateTo(PageRoute.Auth);
	}
}