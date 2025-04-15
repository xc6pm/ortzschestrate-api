using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ortzschestrate.Api.Security;
using Ortzschestrate.Data.Models;
using Ortzschestrate.Infrastructure;

namespace Ortzschestrate.Api.Controllers;

[Route("/api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private const string ValidUsernameChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@";

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
            return Results.Problem("All fields must have a value!");
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
            return Results.Problem("A user with this email/username doesn't exist; try registering first");
        }

        // The user logged in with Google or something and has no password, we can't log in by username/password here
        if (user.PasswordHash == null)
        {
            return Results.Problem("There's no password associated with that account.");
        }

        if (passwordHasher.VerifyHashedPassword(
                user, user.PasswordHash, creds.Password) ==
            PasswordVerificationResult.Failed)
        {
            return Results.Problem("Incorrect password.");
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
            return Results.Problem("All fields must have a value!");
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
            return Results.Problem(createUserResult.Errors.First().Description);
        }

        authenticationHelper.AppendUserTokens(newUser.Id, Response);
        return Results.Ok();
    }

    [ActionName("google")]
    public IResult LoginWithGoogle([FromQuery] string redirect) =>
        Results.Challenge(new() { RedirectUri = redirect },
            [GoogleDefaults.AuthenticationScheme]);

    [ActionName("google-token")]
    [HttpPost]
    public async Task<IResult> GetGoogleToken(
        [FromServices] IHttpClientFactory clientFactory,
        [FromServices] UserManager<User> userManager,
        [FromServices] AuthenticationHelper authenticationHelper)
    {
        Console.WriteLine("Get Google Token");
        Console.WriteLine(
            $"Params received {Request.Form["grant_type"]}; {Request.Form["client_id"]}; {Request.Form["code_verifier"]}; {Request.Form["redirect_uri"]}; {Request.Form["code"]}");

        using var httpClient = clientFactory.CreateClient();
        var clientSecret = Environment.GetEnvironmentVariable(EnvKeys.GoogleClientSecret)!;
        var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token",
            new FormUrlEncodedContent(
            [
                new("client_id", Request.Form["client_id"]),
                new("client_secret", clientSecret),
                new("code", Request.Form["code"]),
                new("code_verifier", Request.Form["code_verifier"]),
                new("grant_type", Request.Form["grant_type"]),
                new("redirectUri", Request.Form["redirect_uri"]),
            ]));
        var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>(new JsonSerializerOptions()
            { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });

        Console.WriteLine("response received " + response.StatusCode);
        Console.WriteLine("response reason phrase " + response.ReasonPhrase);
        Console.WriteLine(
            $"TokenResponse {tokenResponse.RefreshToken}; {tokenResponse.Scope}; {tokenResponse.AccessToken}; {tokenResponse.ExpiresIn}");

        var userInfoResponse =
            await httpClient.GetAsync(
                $"https://www.googleapis.com/oauth2/v1/userinfo?access_token={tokenResponse.AccessToken}");

        Console.WriteLine("user info response received " + userInfoResponse.StatusCode);
        var userInfo = await userInfoResponse.Content.ReadFromJsonAsync<GoogleUserInfo>(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });

        Console.WriteLine("userInfo values: ");
        Console.WriteLine($"{userInfo.Email} {userInfo.Name}");


        var res = await LoginOrRegisterWithGoogle(userInfo, Request.Form["redirect_uri"]!, userManager,
            authenticationHelper);
        Console.WriteLine("res " + res);
        return res;
    }

    [HttpPost]
    public IResult RenewTokens(AuthenticationHelper authenticationHelper, HttpContext httpContext) =>
        authenticationHelper.ExtendUserSession(httpContext);

    [ActionName("user")]
    [Authorize]
    public async Task<IResult> GetUserInfoAsync([FromServices] UserManager<User> userManager)
    {
        Console.WriteLine("Get User Info");
        var user = await userManager.FindByIdAsync(HttpContext.User.FindId());
        Console.WriteLine($"Found user name {user?.UserName}");
        return Results.Ok(new { user!.Id, user.UserName, user.Email, VerifiedWallet = user.WalletAddress });
    }

    [HttpPost]
    [Authorize]
    public IResult Logout([FromServices] JwtIntoCookieInjector injector)
    {
        injector.RemoveTokens(Response);
        return Results.Ok();
    }

    private async Task<IResult> LoginOrRegisterWithGoogle(GoogleUserInfo userInfo, string returnUri,
        UserManager<User> userManager,
        AuthenticationHelper authenticationHelper)
    {
        Console.WriteLine("Inside the method");
        var userWithThisEmail = await userManager.FindByEmailAsync(userInfo.Email);
        Console.WriteLine("Found user with email: " + userWithThisEmail);
        if (userWithThisEmail != null)
        {
            Console.WriteLine("Found by email; appending tokens");
            authenticationHelper.AppendUserTokens(userWithThisEmail.Id, Response);
            Console.WriteLine("Redirecting...");
            Console.WriteLine($"Tokens: {Response.Cookies}");
            return Results.Ok();
        }


        Console.WriteLine("Creating new user");

        var newUser = new User()
        {
            Email = userInfo.Email,
            UserName = userInfo.Name.Replace(" ", "")
        };

        var result = await userManager.CreateAsync(newUser);
        if (!result.Succeeded)
        {
            if (result.Errors.Take(2).Count() == 1 && (result.Errors.First().Code == "InvalidUsername" ||
                                                       result.Errors.First().Code == "DuplicateUsername"))
            {
                var googleNameContainsInvalidChars = newUser.UserName.Any(ch => !ValidUsernameChars.Contains(ch));
                var usernameBase = googleNameContainsInvalidChars ? newUser.Email.Split('@')[0] : newUser.UserName;
                bool shouldTryEmailWithoutGuidFirst = googleNameContainsInvalidChars;

                do
                {
                    newUser.UserName = usernameBase +
                                       (shouldTryEmailWithoutGuidFirst
                                           ? ""
                                           : Guid.NewGuid().ToString("N").Substring(0, 4));
                    shouldTryEmailWithoutGuidFirst = false;
                    result = await userManager.CreateAsync(newUser);
                } while (!result.Succeeded && (result.Errors.First().Code == "InvalidUsername" ||
                                               result.Errors.First().Code == "DuplicateUsername"));

                if (!result.Succeeded)
                {
                    // Fail(string.Join(" --- ", result.Errors));
                    return Results.Problem();
                }
            }
            else
            {
                // Fail(string.Join(" --- ", result.Errors));
                return Results.Problem();
            }
        }

        authenticationHelper.AppendUserTokens(newUser.Id, Response);
        return Results.Redirect(returnUri);
    }

    public record AuthReq(string Email, string Password, string Username);

    record GoogleTokenResponse(string AccessToken, string RefreshToken, int ExpiresIn, string Scope);

    record GoogleUserInfo(
        string Id,
        string Email,
        bool VerifiedEmail,
        string Name,
        string GivenName,
        string FamilyName,
        string Picture);
}