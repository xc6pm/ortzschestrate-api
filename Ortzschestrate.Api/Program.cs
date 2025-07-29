using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Bytewizer.Backblaze.Client;
using FluentFTP;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ortzschestrate.Api.Hubs;
using Ortzschestrate.Api.Hubs.Game;
using Ortzschestrate.Api.Security;
using Ortzschestrate.Api.Utilities;
using Ortzschestrate.Data.Models;
using Ortzschestrate.Infrastructure;
using Ortzschestrate.Web3;
using DbContext = Ortzschestrate.Data.DbContext;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddDbContext<DbContext>(options =>
{
    string connectionString = Environment.GetEnvironmentVariable(EnvKeys.ConnectionString) ??
                              throw new Exception("Connection string not found.");
    options.UseNpgsql(connectionString);
});

const string validUsernameChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

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

        // options.CallbackPath = "/api/auth/google-cb";

        options.Events.OnTicketReceived = async context =>
        {
            Console.WriteLine("OnTicketReceived");

            // Need to handle this because UserManager.SignInAsync doesn't work with Jwt bearer.
            context.HandleResponse();

            Console.WriteLine("Response handled");

            var email = context.Principal!.Claims.First(c => c.Type == ClaimTypes.Email).Value.Trim();
            var name = context.Principal!.Claims.First(c => c.Type == ClaimTypes.Name).Value.Trim();
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
            var authenticationHelper = context.HttpContext.RequestServices.GetRequiredService<AuthenticationHelper>();

            Console.WriteLine("Ingredients resolved");

            var userWithThisEmail = await userManager.FindByEmailAsync(email);
            if (userWithThisEmail != null)
            {
                Console.WriteLine("Found by email; appending tokens");
                authenticationHelper.AppendUserTokens(userWithThisEmail.Id, context.Response);
                Console.WriteLine("Redirecting...");
                context.Response.Redirect(context.ReturnUri);
                return;
            }


            Console.WriteLine("Creating new user");

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
        options.User.AllowedUserNameCharacters = validUsernameChars;
    })
    .AddEntityFrameworkStores<DbContext>()
    .AddDefaultTokenProviders()
    .AddTokenProvider<WalletVerificationTokenProvider<User>>(WalletVerificationTokenProvider<User>.Key);

builder.Services.AddSignalR();

string corsPolicyName = "cors_whitelist";
string[] corsWhitelist = (Environment.GetEnvironmentVariable(EnvKeys.CorsWhiteList) ?? "").Split(";");
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName,
        b => b.WithOrigins(corsWhitelist).AllowAnyHeader().AllowCredentials());
});


#if DEBUG
builder.Services.AddDataProtection()
    .SetApplicationName("Ortzschestrate")
    .PersistKeysToFileSystem(
        new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Keys")));
#else
using (var client = new BackblazeClient())
{
    string backblazeKeyId = Environment.GetEnvironmentVariable(EnvKeys.BackblazeKeyId) ??
                            throw new Exception("Backblaze Key Id is required to start the application.");
    string backblazeApplicationKey = Environment.GetEnvironmentVariable(EnvKeys.BackblazeApplicationKey) ??
                                     throw new Exception(
                                         "Backblaze Application Key is required to start the application.");

    client.Connect(backblazeKeyId, backblazeApplicationKey);
    var buckets = await client.Buckets.GetAsync();

    var bucket = buckets.First(b => b.BucketId == "f1e2166cbb3a47be9d85061d");

    var certStream = new MemoryStream();
    var results = await client.DownloadAsync(bucket.BucketName, "dataprotection.pfx", certStream);
    results.EnsureSuccessStatusCode();

    string dataProtectionCertPass = Environment.GetEnvironmentVariable(EnvKeys.DataProtectionCertPass) ??
                                    throw new Exception(
                                        $"{EnvKeys.DataProtectionCertPass} is required to start the application.");
    
    var dataProtectionCert = new X509Certificate2(certStream.ToArray(), dataProtectionCertPass.TrimEnd('\n'));
    builder.Services.AddDataProtection()
        .SetApplicationName("Ortzschestrate")
        .PersistKeysToDbContext<DbContext>()
        .ProtectKeysWithCertificate(dataProtectionCert);
}
#endif

ServiceRegisterer.RegisterServices(builder.Services);

builder.Services.AddSingleton<IOutgoingMessageTracker, OutgoingMessageTracker>();

builder.Services.AddTransient<WalletVerificationTokenProvider<User>>();

builder.Services.AddSingleton<JwtIntoCookieInjector>();
builder.Services.AddSingleton<AuthenticationHelper>();

builder.Services.AddSingleton<PlayerCache>();
builder.Services.AddSingleton<EmailSender>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(corsPolicyName);

app.UseHttpsRedirection();

// This is to make Google to redirect to the https endpoint of the api.
var forwardedHeaderOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedHeaderOptions.KnownNetworks.Clear();
forwardedHeaderOptions.KnownProxies.Clear();

app.UseForwardedHeaders(forwardedHeaderOptions);

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<GameHub>("/hubs/game");


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/appdata", () => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)))
    .WithName("GetAppData").WithOpenApi();

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