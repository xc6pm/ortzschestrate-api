using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Ortzschestrate.Api.Security;
using Ortzschestrate.Data;
using Ortzschestrate.Data.Models;
using Ortzschestrate.Utilities.Security;

namespace Ortzschestrate.Api.Controllers;

[Route("/api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    [HttpPost]
    public async Task<IResult> LoginAsync(
        [FromServices] UserManager<User> userManager,
        [FromServices] IPasswordHasher<User> passwordHasher,
        [FromServices] AuthenticationHelper authenticationHelper)
    {
        var user = extractAuthorizationInformationFromHeader();
        var userWithThisEmail = await findUserByEmail(user.Email, userManager);
        if (userWithThisEmail == null)
        {
            return Results.NotFound("A user with this email doesn't exist; try registering first");
        }

        // The user logged in with Google or something and has no password, we can't log in by username/password here
        if (userWithThisEmail.PasswordHash == null)
        {
            return Results.Unauthorized();
        }

        if (passwordHasher.VerifyHashedPassword(
                userWithThisEmail, userWithThisEmail.PasswordHash, user.Password) ==
            PasswordVerificationResult.Failed)
        {
            return Results.Unauthorized();
        }

        authenticationHelper.AppendUserTokens(userWithThisEmail.Id, Response);
        return Results.Ok();
    }

    public async Task<IResult> RegisterAsync(
        [FromServices] UserManager<User> userManager,
        [FromServices] DbContext dbContext,
        [FromServices] AuthenticationHelper authenticationHelper)
    {
        var user = extractAuthorizationInformationFromHeader();
        var userWithThisEmail = await findUserByEmail(user.Email, userManager);
        if (userWithThisEmail != null)
        {
            return Results.Conflict("A user with this email already exists");
        }

        string normalizedEmail = user.Email.ToUpperInvariant();
        var newUser = new User()
        {
            Email = user.Email,
            NormalizedEmail = normalizedEmail
        };

        dbContext.Users.Add(newUser);
        await userManager.AddPasswordAsync(newUser, user.Password);
        await dbContext.SaveChangesAsync();

        authenticationHelper.AppendUserTokens(newUser.Id, Response);
        return Results.Ok();
    }

    [ActionName("google")]
    public IResult LoginWithGoogle() =>
        Results.Challenge(new() { RedirectUri = "/" }, [GoogleDefaults.AuthenticationScheme]);

    [HttpPost]
    public IResult RenewTokens(AuthenticationHelper authenticationHelper, HttpContext httpContext) =>
        authenticationHelper.ExtendUserSession(httpContext);

    [ActionName("user")]
    [Authorize]
    public IResult GetCurrentUser() =>
        Results.Json(HttpContext.User.Claims.Select(c => new { c.Type, c.Value }).ToList());

    private AuthorizationInformation extractAuthorizationInformationFromHeader()
    {
        string encodedUsernamePassword =
            Request.Headers.Authorization[0]!.Split(' ')[1];
        string decoded = Base64UrlEncoder.Decode(encodedUsernamePassword);

        // We expect an email instead of username often provided with basic authentication scheme.
        string[] emailAndPassword = decoded.Split(':');

        return new AuthorizationInformation(emailAndPassword[0], emailAndPassword[1]);
    }

    private async Task<User?> findUserByEmail(
        string email, UserManager<User> userManager)
    {
        if (!EmailValidator.IsValid(email))
        {
            return null;
        }

        var userWithThisEmail = await userManager.FindByEmailAsync(email);
        return userWithThisEmail;
    }

    private record AuthorizationInformation(string Email, string Password);
}