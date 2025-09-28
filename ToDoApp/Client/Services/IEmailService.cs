using Refit;
using ToDoApp.Shared;

namespace ToDoApp.Client.Services;

public interface IEmailService
{
	[Post(ApiEndpoints.EmailEndpoints.Send)]
	Task<ApiResponse<bool>> Send();
}