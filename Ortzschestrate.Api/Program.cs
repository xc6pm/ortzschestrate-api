using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ortzschestrate.Api.Security;
using Ortzschestrate.Data.Models;
using DbContext = Ortzschestrate.Data.DbContext;

var builder = WebApplication.CreateBuilder(args);

await Task.Delay(4000);

builder.Services.AddDbContext<DbContext>(options =>
{
    string connectionString = Environment.GetEnvironmentVariable("ORTZSCHESTRATE_CONNECTION_STRING") ??
        throw new Exception("Connection string not found.");
    Debug.WriteLine(connectionString);
    options.UseNpgsql(connectionString);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
#if DEBUG
        options.RequireHttpsMetadata = false;
#endif

        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = false,
            ValidIssuer = JwtGenerator.ValidIssuer,
            ClockSkew = TimeSpan.FromSeconds(0),
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("PB_JWT_SECRET")!))
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

        options.ClientId = Environment.GetEnvironmentVariable("ORTSCHESTRATE_GOOGLE_CLIENT_ID");
        options.ClientSecret = Environment.GetEnvironmentVariable("ORTSCHESTRATE_GOOGLE_CLIENT_SECRET");

        options.CallbackPath = "/auth/google-cb";

        options.Events.OnTicketReceived = async context =>
        {
            // Need to handle this because UserManager.SignInAsync doesn't work with Jwt bearer.
            context.HandleResponse();

            var email = context.Principal!.Claims.First(c => c.Type == ClaimTypes.Email).Value;
            var dbContext = context.HttpContext.RequestServices.GetRequiredService<DbContext>();
            var authenticationHelper = context.HttpContext.RequestServices.GetRequiredService<AuthenticationHelper>();

            var normalizedEmail = email.ToUpperInvariant();
            var userWithThisEmail = dbContext.Users.FirstOrDefault(u => u.NormalizedEmail == normalizedEmail);
            if (userWithThisEmail != null)
            {
                authenticationHelper.AppendUserTokens(userWithThisEmail.Id, context.Response);
                context.Response.Redirect("/");
                return;
            }

            var newUser = new User()
            {
                Email = email,
                NormalizedEmail = normalizedEmail,
            };

            dbContext.Users.Add(newUser);
            await dbContext.SaveChangesAsync();

            authenticationHelper.AppendUserTokens(newUser.Id, context.Response);
            context.Response.Redirect("/");
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddIdentityCore<User>(options => { options.User.RequireUniqueEmail = true; })
    .AddEntityFrameworkStores<DbContext>()
    .AddDefaultTokenProviders();

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
}


app.UseHttpsRedirection();

app.UseAuthorization();


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
