using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ortzschestrate.Api.Security;
using Ortzschestrate.Data.Models;
using Ortzschestrate.Utilities.Security;

namespace Ortzschestrate.Api.Controllers;

[Route("/api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    [HttpPost]
    public async Task<IResult> LoginAsync(
        [FromBody] AuthReq creds,
        [FromServices] UserManager<User> userManager,
        [FromServices] IPasswordHasher<User> passwordHasher,
        [FromServices] AuthenticationHelper authenticationHelper)
    {
        if ((String.IsNullOrWhiteSpace(creds.Email) && String.IsNullOrWhiteSpace(creds.Username)) ||
            String.IsNullOrWhiteSpace(creds.Password))
        {
            return Results.BadRequest("All fields must have a value.");
        }

        User? user;
        if (!String.IsNullOrWhiteSpace(creds.Email))
        {
            user = await userManager.FindByEmailAsync(creds.Email);
        }
        else
        {
            user = await userManager.FindByNameAsync(creds.Username);
        }

        if (user == null)
        {
            return Results.NotFound("A user with this email/username doesn't exist; try registering first");
        }

        // The user logged in with Google or something and has no password, we can't log in by username/password here
        if (user.PasswordHash == null)
        {
            return Results.Unauthorized();
        }

        if (passwordHasher.VerifyHashedPassword(
                user, user.PasswordHash, creds.Password) ==
            PasswordVerificationResult.Failed)
        {
            return Results.Unauthorized();
        }

        authenticationHelper.AppendUserTokens(user.Id, Response);
        return Results.Ok();
    }

    [HttpPost]
    public async Task<IResult> RegisterAsync(
        [FromBody] AuthReq creds,
        [FromServices] UserManager<User> userManager,
        [FromServices] AuthenticationHelper authenticationHelper)
    {
        string[] values = [creds.Email, creds.Password, creds.Username];
        if (values.Any(String.IsNullOrWhiteSpace))
        {
            return Results.BadRequest("All fields must have a value!");
        }

        var newUser = new User()
        {
            Email = creds.Email,
            UserName = creds.Username
        };

        var createUserResult = await userManager.CreateAsync(newUser, creds.Password);

        if (!createUserResult.Succeeded)
        {
            // Existing email/username, and invalid (easy) passwords are within the Errors.
            return Results.BadRequest(new
            {
                Errors = createUserResult.Errors.Select(error => error.Description).ToArray()
            });
        }

        authenticationHelper.AppendUserTokens(newUser.Id, Response);
        return Results.Ok();
    }

    [ActionName("google")]
    public IResult LoginWithGoogle([FromQuery] string redirect) =>
        Results.Challenge(new() { RedirectUri = redirect },
            [GoogleDefaults.AuthenticationScheme]);

    [HttpPost]
    public IResult RenewTokens(AuthenticationHelper authenticationHelper, HttpContext httpContext) =>
        authenticationHelper.ExtendUserSession(httpContext);

    [ActionName("user")]
    [Authorize]
    public async Task<IResult> GetUserInfo([FromServices] UserManager<User> userManager)
    {
        var user = await userManager.FindByIdAsync(HttpContext.User.Claims
            .First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        return Results.Ok(new { user!.UserName, user.Email });
    }

    public record AuthReq(string Email, string Password, string Username);

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
}