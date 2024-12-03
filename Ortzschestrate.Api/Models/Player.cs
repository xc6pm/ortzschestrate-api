using System.Collections.Concurrent;
using System.Text.Json.Serialization;

namespace Ortzschestrate.Api.Models;

public class Player(string userId, string name)
{
    public string UserId { get; } = userId;
    public string Name { get; } = name;

    /// <summary>
    /// Ongoing games by the connectionId of this player (not the creator).
    /// </summary>
    [JsonIgnore]
    public ConcurrentDictionary<string, Game> OngoingGamesByConnectionId { get; } = new();
}