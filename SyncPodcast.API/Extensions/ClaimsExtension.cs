using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
namespace SyncPodcast.API.Extensions;

public static class ClaimsExtension
{
    public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var claim = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sub)
             ?? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);

        return Guid.Parse(claim!.Value);
    }
}
