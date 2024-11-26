using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ortzschestrate.Api.Hubs;
using Ortzschestrate.Api.Security;
using Ortzschestrate.Api.Utilities;
using Ortzschestrate.Data.Models;
using DbContext = Ortzschestrate.Data.DbContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbContext>(options =>
{
    string connectionString = Environment.GetEnvironmentVariable(EnvKeys.ConnectionString) ??
                              throw new Exception("Connection string not found.");
    options.UseNpgsql(connectionString);
});

string validUsernameChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.SaveToken = true;
        // Without this it changes the claim name from sub to username or something when we receive tokens.
        // https://stackoverflow.com/q/57998262
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = false,
            ValidIssuer = JwtGenerator.ValidIssuer,
            ClockSkew = TimeSpan.FromSeconds(0),
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable(EnvKeys.JwtSecret)!))
        };

        options.Events = new JwtBearerEvents()
        {
            OnMessageReceived = messageReceivedContext =>
            {
                // The authentication middleware looks for the cookie in the authorization header by default,
                // we need to extract it from the cookies.
                var authenticationHelper = messageReceivedContext.HttpContext.RequestServices
                    .GetRequiredService<AuthenticationHelper>();
                messageReceivedContext.Token = authenticationHelper.ReadToken(messageReceivedContext.Request);
                return Task.CompletedTask;
            }
        };
    })
    .AddGoogle(options =>
    {
        options.SignInScheme = JwtBearerDefaults.AuthenticationScheme;

        options.ClientId = Environment.GetEnvironmentVariable(EnvKeys.GoogleClientId) ??
                           throw new InvalidOperationException(
                               "The google client id variable must be present or google authentication won't work!");
        options.ClientSecret = Environment.GetEnvironmentVariable(EnvKeys.GoogleClientSecret) ??
                               throw new InvalidOperationException(
                                   "The google client secret variable must be present or google authentication won't work!");

        options.CallbackPath = "/api/auth/google-cb";

        options.Events.OnTicketReceived = async context =>
        {
            // Need to handle this because UserManager.SignInAsync doesn't work with Jwt bearer.
            context.HandleResponse();

            var email = context.Principal!.Claims.First(c => c.Type == ClaimTypes.Email).Value.Trim();
            var name = context.Principal!.Claims.First(c => c.Type == ClaimTypes.Name).Value.Trim();
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
            var authenticationHelper = context.HttpContext.RequestServices.GetRequiredService<AuthenticationHelper>();

            var userWithThisEmail = await userManager.FindByEmailAsync(email);
            if (userWithThisEmail != null)
            {
                authenticationHelper.AppendUserTokens(userWithThisEmail.Id, context.Response);
                context.Response.Redirect(context.ReturnUri);
                return;
            }

            var newUser = new User()
            {
                Email = email,
                UserName = name.Replace(" ", "")
            };

            var result = await userManager.CreateAsync(newUser);
            if (!result.Succeeded)
            {
                if (result.Errors.Take(2).Count() == 1 && (result.Errors.First().Code == "InvalidUsername" ||
                                                           result.Errors.First().Code == "DuplicateUsername"))
                {
                    var googleNameContainsInvalidChars = newUser.UserName.Any(ch => !validUsernameChars.Contains(ch));
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
                        context.Fail(string.Join(" --- ", result.Errors));
                        return;
                    }
                }
                else
                {
                    context.Fail(string.Join(" --- ", result.Errors));
                    return;
                }
            }

            authenticationHelper.AppendUserTokens(newUser.Id, context.Response);
            context.Response.Redirect(context.ReturnUri);
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddIdentityCore<User>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    })
    .AddEntityFrameworkStores<DbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSignalR();

string devCorsPolicyName = "local dev client";
builder.Services.AddCors(options =>
{
    options.AddPolicy(devCorsPolicyName,
        builder => builder.WithOrigins("https://localhost:3000").AllowAnyHeader().AllowCredentials());
});


builder.Services.AddDataProtection();


builder.Services.AddSingleton<JwtIntoCookieInjector>();
builder.Services.AddSingleton<AuthenticationHelper>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(devCorsPolicyName);
}


app.UseHttpsRedirection();


app.UseAuthorization();

app.MapHub<GameHub>("/hubs/game");


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();


app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}