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
    public async Task<IResult> LoginAsync(HttpContext httpContext,
        DbContext dbContext,
        UserManager<User> userManager,
        [FromServices] IPasswordHasher<User> passwordHasher,
        AuthenticationHelper authenticationHelper)
    {
        string encodedUsernamePassword =
            httpContext.Request.Headers.Authorization[0]!.Split(' ')[1];
        string decoded = Base64UrlEncoder.Decode(encodedUsernamePassword);

        // We expect an email instead of username often provided with basic authentication scheme.
        string[] emailAndPassword = decoded.Split(':');

        if (!EmailValidator.IsValid(emailAndPassword[0]))
        {
            return Results.BadRequest($"{emailAndPassword[0]} is an invalid email");
        }

        string normalizedEmail = emailAndPassword[0].ToUpperInvariant();

        var userWithThisEmail = await userManager.FindByEmailAsync(normalizedEmail);
        if (userWithThisEmail == null) // Create the user.
        {
            var newUser = new User()
            {
                Email = emailAndPassword[0],
                NormalizedEmail = normalizedEmail
            };

            dbContext.Users.Add(newUser);
            await userManager.AddPasswordAsync(newUser, emailAndPassword[1]);
            await dbContext.SaveChangesAsync();

            authenticationHelper.AppendUserTokens(newUser.Id, httpContext.Response);
            return Results.Ok();
        }

        if (passwordHasher.VerifyHashedPassword(
                userWithThisEmail, userWithThisEmail.PasswordHash, emailAndPassword[1]) ==
            PasswordVerificationResult.Failed)
        {
            return Results.Unauthorized();
        }

        authenticationHelper.AppendUserTokens(userWithThisEmail.Id, httpContext.Response);
        return Results.Ok();
    }

    [ActionName("google")]
    public IResult LoginWithGoogle() => Results.Challenge(new() { RedirectUri = "/" }, [GoogleDefaults.AuthenticationScheme]);

    [HttpPost]
    public IResult RenewTokens(AuthenticationHelper authenticationHelper, HttpContext httpContext) => 
        authenticationHelper.ExtendUserSession(httpContext);

    [HttpGet(Name = "user")]
    [Authorize]
    public IResult GetCurrentUser(HttpContext httpContext) => Results.Json(httpContext.User.Claims.Select(c => new {c.Type, c.Value}).ToList());
}
