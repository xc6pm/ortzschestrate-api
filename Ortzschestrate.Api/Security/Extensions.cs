using System.Security.Claims;

namespace Ortzschestrate.Api.Security;

public static class Extensions
{
    public static string ExtractId(this ClaimsPrincipal user) =>
        user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
}