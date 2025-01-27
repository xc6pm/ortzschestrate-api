using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ortzschestrate.Api.Security;
using Ortzschestrate.Api.Utilities;
using Ortzschestrate.Data.Models;
using Ortzschestrate.Web3.Utilities;
using DbContext = Ortzschestrate.Data.DbContext;

namespace Ortzschestrate.Api.Controllers;

[Route("/api/[controller]/[action]")]
public class WalletController : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IResult> Verify(string walletAddress, [FromServices] DbContext dbContext,
        [FromServices] UserManager<User> userManager, [FromServices] EmailSender emailSender)
    {
        if (!Validator.IsValidEthereumAddressHexFormat(walletAddress))
            return Results.ValidationProblem(new Dictionary<string, string[]>()
            {
                { nameof(walletAddress), ["Invalid Ethereum address."] }
            });

        var userId = HttpContext.User.FindId();
        var user = (await dbContext.Users.FindAsync(userId))!;
        user.UnverifiedWalletAddress = walletAddress;
        await dbContext.SaveChangesAsync();

        var token = await generateWalletVerificationTokenAsync(userManager, user);
        var link = Url.ActionLink(nameof(Confirm), null, new { token, email = user.Email });

        await emailSender.SendWalletVerificationEmailAsync(user.Email!, link!);
        return Results.Ok();
    }

    [HttpGet]
    public async Task<IResult> Confirm(string token, string email, [FromServices] UserManager<User> userManager,
        [FromServices] DbContext dbContext)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
            return Results.Unauthorized();

        var verified = await userManager.VerifyUserTokenAsync(user, WalletVerificationTokenProvider<User>.Key,
            WalletVerificationTokenProvider<User>.Purpose, token);

        if (!verified)
            return Results.BadRequest("Wallet verification failed. Try another time.");

        var userInDb = await dbContext.Users.FindAsync(user.Id);
        userInDb!.WalletAddress = user.UnverifiedWalletAddress;
        userInDb.UnverifiedWalletAddress = string.Empty;
        await dbContext.SaveChangesAsync();

        return Results.Ok();
    }

    private Task<string> generateWalletVerificationTokenAsync(UserManager<User> userManager, User user) =>
        userManager.GenerateUserTokenAsync(user, WalletVerificationTokenProvider<User>.Key,
            WalletVerificationTokenProvider<User>.Purpose);
}