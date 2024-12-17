using System.Collections.Concurrent;
using System.Text.Json.Serialization;

namespace Ortzschestrate.Api.Models;

public class Player(string userId, string name)
{
    public string UserId { get; } = userId;
    public string Name { get; } = name;

    // I may add long games (like 3 days) too, but for now the player should be able to play one short game at a time. 
    [JsonIgnore]
    public Game? OngoingShortGame { get; set; }
}