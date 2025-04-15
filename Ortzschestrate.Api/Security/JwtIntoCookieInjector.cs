using Microsoft.AspNetCore.DataProtection;
using CookieOptions = Microsoft.AspNetCore.Http.CookieOptions;

namespace Ortzschestrate.Api.Security;

public class JwtIntoCookieInjector(IDataProtectionProvider protectorProvider)
{
    public const string TokenCookieKey = "g08bzgprar";
    public const string RefreshTokenCookieKey = "b1puvapri2";

    private readonly IDataProtector _tokenProtector =
        protectorProvider.CreateProtector(TokenCookieKey);

    private readonly IDataProtector _refreshTokenProtector =
        protectorProvider.CreateProtector(RefreshTokenCookieKey);

    private readonly CookieOptions _cookieOptions = new()
    {
        HttpOnly = true,
        SameSite = SameSiteMode.None,
        Secure = true
    };

    public void InjectTokens(IssuedTokenResult tokens, HttpResponse response)
    {
        response.Cookies.Append(TokenCookieKey, _tokenProtector.Protect(tokens.Token), _cookieOptions);
        response.Cookies.Append(RefreshTokenCookieKey, _refreshTokenProtector.Protect(tokens.RefreshToken),
            _cookieOptions);
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
        response.Cookies.Delete(TokenCookieKey, _cookieOptions);
        response.Cookies.Delete(RefreshTokenCookieKey, _cookieOptions);
    }
}