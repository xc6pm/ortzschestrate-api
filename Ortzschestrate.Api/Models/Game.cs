namespace Ortzschestrate.Api.Models;

public record Game(string Player1, string Player2)
{
    public readonly string Id = Guid.NewGuid().ToString();
}