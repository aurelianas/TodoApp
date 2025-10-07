using System.Security.Claims;

namespace ToDoApp.Shared.Utils;

public static class ClaimsPrincipalCustom
{
    public static int GetUserProfileId(this ClaimsPrincipal user)
    {
        if (user is ClaimsPrincipal && user.Identity!.IsAuthenticated)
        {
            var userProfileIdClaim = user.FindFirst("userProfileId");
            if (userProfileIdClaim != null)
            {
                return int.Parse(userProfileIdClaim.Value);
            }
        }

        return 0;
    }

	public static string? GetRefreshTokenId(this ClaimsPrincipal user)
	{
		if (user is ClaimsPrincipal && user.Identity!.IsAuthenticated)
		{
			var userProfileIdClaim = user.FindFirst("refreshTokenId");

			if (userProfileIdClaim is not null)
			{
				return userProfileIdClaim.Value;
			}
		}

		return null;
	}
}