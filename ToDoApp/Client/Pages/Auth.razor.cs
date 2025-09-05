using Blazored.LocalStorage;
using ChangeTracking;
using Microsoft.AspNetCore.Components;
using SpawnDev.BlazorJS.Cryptography;
using System.Net;
using System.Text;
using ToDoApp.Client.Authentication;
using ToDoApp.Client.Services;
using ToDoApp.Shared.Models;
using ToDoApp.Shared.Validations;

namespace ToDoApp.Client.Pages;

[Route($"{PageRoute.Auth}")]
public partial class Auth
{
	#region Fields

	private FluentValidationValidator? _fluentValidationValidator;
	private AuthModel _model;

	#endregion //Fields

	#region Private Properties

	[Inject] private IUserCredentialService? UserCredentialService { get; set; }

	[Inject] private IAuthService? AuthService { get; set; }

	[Inject] private CustomAuthenticationStateProvider CustomAuthenticationStateProvider { get; set; }

	[Inject] private NavigationManager NavigationManager { get; set; }

	[Inject] private ILocalStorageService LocalStorageService { get; set; }

	[Inject] private BrowserCrypto BrowserCrypto { get; set; }

	private AuthModel Model
	{
		get
		{
			if (_model is null)
			{
				Model = new();
			}

			return _model!;
		}

		set
		{
			_model = value;

			if (_model is not IChangeTrackable)
			{
				_model = _model.AsTrackable();
			}
		}
	}

	private bool ShowRegister { get; set; }

	private bool ShowPassword { get; set; }

	private string? ValidationError { get; set; }

	#endregion Private Properties

	#region Private Methods

	private async Task<string> HashPassword(string password)
	{
		var data = Encoding.UTF8.GetBytes(password);
		var passwordHash = await BrowserCrypto.Digest("SHA-256", data);
		return Convert.ToBase64String(passwordHash);
	}

	private async Task Login()
	{
		var requestModel = new AuthModel()
		{
			UserName = Model.UserName,
			Password = await HashPassword(Model.Password)
		};

		var result = await AuthService!.Login(requestModel);

		if (result.StatusCode is HttpStatusCode.BadRequest)
		{
			ValidationError = "User or password are incorrect!";
			return;
		}

		ValidationError = null;
		await CustomAuthenticationStateProvider.MarkUserAsAuthenticated(result.Content);
		NavigationManager.NavigateTo("/");
	}

	private async Task Register()
	{
		var requestModel = new AuthModel()
		{
			UserName = Model.UserName,
			Password = await HashPassword(Model.Password)
		};

		var result = await AuthService!.Register(requestModel);

		if (result.StatusCode is HttpStatusCode.BadRequest)
		{
			ValidationError = "User already exists!";
		}

		if (result.IsSuccessStatusCode)
		{
			ShowRegister = false;
			ShowPassword = false;
			ValidationError = null;
			Model = new();
		}
	}

	private void TooglePasswordVisibility()
	{
		ShowPassword = !ShowPassword;
	}

	private void CreateAccount()
	{
		ShowRegister = true;
		ShowPassword = false;
		ValidationError = null;
		Model = new();
	}

	private void GoToLogin()
	{
		ShowRegister = false;
		ShowPassword = false;
		ValidationError = null;
		Model = new();
	}

	#endregion Private Methods
}