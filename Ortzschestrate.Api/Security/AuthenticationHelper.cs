namespace Ortzschestrate.Api.Security;

public class AuthenticationHelper(JwtIntoCookieInjector jwtIntoCookieInjector)
{
    public void AppendUserTokens(string userId, HttpResponse response)
    {
        var tokens = JwtGenerator.GenerateAuthAndRefreshTokens(userId);
        jwtIntoCookieInjector.InjectTokens(tokens, response);
    }

    public string? ReadToken(HttpRequest request)
    {
        return jwtIntoCookieInjector.ReadToken(request);
    }

    public IResult ExtendUserSession(HttpContext context)
    {
        var refreshToken = jwtIntoCookieInjector.ReadRefreshToken(context.Request);
        if (!JwtGenerator.ValidateRefreshToken(refreshToken, out string userId))
        {
            return Results.Unauthorized();
        }

        var newTokens = JwtGenerator.GenerateAuthAndRefreshTokens(userId);
        jwtIntoCookieInjector.InjectTokens(newTokens, context.Response);
        return Results.Ok();
    }
}