using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Ortzschestrate.Infrastructure;

namespace Ortzschestrate.Api.Security;

public class JwtGenerator
{
    public const string ValidIssuer = "Ortzschestrate";

    private static readonly SigningCredentials authTokenSigningCredentials = new(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable(EnvKeys.JwtSecret)!)),
        SecurityAlgorithms.HmacSha256);

    private static readonly SigningCredentials refreshTokenSigningCredentials = new(
        new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable(EnvKeys.JwtRefreshSecret)!)),
        SecurityAlgorithms.HmacSha256);

    public static IssuedTokenResult GenerateAuthAndRefreshTokens(string userId) =>
        new(GenerateAuthToken(userId), GenerateRefreshToken(userId));

    public static string GenerateAuthToken(string userId)
    {
        Claim[] claims = [new(JwtRegisteredClaimNames.Sub, userId)];

        return generate(claims, TimeSpan.FromHours(2), authTokenSigningCredentials);
    }

    public static string GenerateRefreshToken(string userId)
    {
        Claim[] claims = [new(JwtRegisteredClaimNames.Sub, userId)];

        return generate(claims, TimeSpan.FromDays(14), refreshTokenSigningCredentials);
    }

    public static bool ValidateRefreshToken(string? refreshToken, out string userId)
    {
        userId = null!;

        if (string.IsNullOrWhiteSpace(refreshToken))
            return false;

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters()
            {
                ValidateAudience = false,
                ValidIssuer = ValidIssuer,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = refreshTokenSigningCredentials.Key,
                ValidateLifetime = true
            }, out SecurityToken validatedToken);

            userId = ((JwtSecurityToken)validatedToken).Subject;

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string generate(IEnumerable<Claim> claims, TimeSpan expiresIn, SigningCredentials signingCredentials)
    {
        var jwtToken = new JwtSecurityToken(ValidIssuer, claims: claims, notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.Add(expiresIn),
            signingCredentials: signingCredentials);

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(jwtToken);
    }
}