using Microsoft.AspNetCore.DataProtection;

namespace Ortzschestrate.Api.Security;

public class JwtIntoCookieInjector(IDataProtectionProvider protectorProvider)
{
    public const string TokenCookieKey = "g08bzgprar";
    public const string RefreshTokenCookieKey = "b1puvapri2";

    private readonly IDataProtector _tokenProtector =
        protectorProvider.CreateProtector(TokenCookieKey);

    private readonly IDataProtector _refreshTokenProtector =
        protectorProvider.CreateProtector(RefreshTokenCookieKey);

    public void InjectTokens(IssuedTokenResult tokens, HttpResponse response)
    {
        var cookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true,
        };

        response.Cookies.Append(TokenCookieKey, _tokenProtector.Protect(tokens.Token), cookieOptions);
        response.Cookies.Append(RefreshTokenCookieKey, _refreshTokenProtector.Protect(tokens.RefreshToken),
            cookieOptions);
    }

    public string? ReadToken(HttpRequest request) =>
        request.Cookies.TryGetValue(TokenCookieKey, out string? value) && value != null
            ? _tokenProtector.Unprotect(value)
            : null;

    public string? ReadRefreshToken(HttpRequest request) =>
        request.Cookies.TryGetValue(RefreshTokenCookieKey, out string? value) && value != null
            ? _refreshTokenProtector.Unprotect(value)
            : null;

    public void RemoveTokens(HttpResponse response)
    {
        response.Cookies.Delete(TokenCookieKey);
        response.Cookies.Delete(RefreshTokenCookieKey);
    }
}