namespace Ortzschestrate.Api.Models;

public class Player(string connectionId, string userId, string name)
{
    public string UserId { get; } = userId;
    public string Name { get; } = name;
    public string ConnectionId { get; } = connectionId;

    public string? GameId { get; set; }
}