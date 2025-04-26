using Ortzschestrate.Api.Models;

namespace Ortzschestrate.Api.Utilities;

public interface IOutgoingMessageTracker
{
    Task GameStartedAsync(string userId, string gameId, int maxRetries = 3, int retryDelayInMs = 1000);

    Task PlayerMovedAsync(string userId, GameUpdate gameUpdate, int maxRetries = 3, int retryDelayInMs = 1000);

    Task OpponentConnectionLostAsync(string userId, int reconnectionTimeout, int maxRetries = 3,
        int retryDelayInMs = 1000);

    Task OpponentReconnectedAsync(string userId, int maxRetries = 3, int retryDelayInMs = 1000);
    Task GameEndedAsync(string userId, GameResult gameResult, int maxRetries = 3, int retryDelayInMs = 1000);

    void Acknowledge(string userId, uint messageId);
}