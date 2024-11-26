using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Ortzschestrate.Api.Utilities;

public static class Extensions
{
    public static string GetSubClaim(this IEnumerable<Claim> claims) =>
        claims.First(c => c.Type == JwtRegisteredClaimNames.Sub)!.Value;

    public static string GetSubClaim(this ClaimsPrincipal cp) => GetSubClaim(cp.Claims);

    public static string? GetSubClaimOrDefault(this IEnumerable<Claim> claims) =>
        claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

    public static string? GetSubClaimOrDefault(this ClaimsPrincipal cp) => GetSubClaimOrDefault(cp.Claims);
}