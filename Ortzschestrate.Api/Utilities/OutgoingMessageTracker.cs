using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Ortzschestrate.Api.Hubs;
using Ortzschestrate.Api.Models;

namespace Ortzschestrate.Api.Utilities;

public class OutgoingMessageTracker(IHubContext<GameHub, IGameClient> hubContext) : IOutgoingMessageTracker
{
    private volatile uint _messageCounter;
    private readonly ConcurrentDictionary<uint, PendingMessage> _pendingMessagesByMessageId = new();
    
    public Task GameStartedAsync(string userId, string gameId, int maxRetries = 3,
        int retryDelayInMs = 1000) =>
        sendWithAck(userId, (messageId) => hubContext.Clients.User(userId)
                .GameStarted(new AckMessage<string>(messageId, gameId)), nameof(IGameClient.GameStarted),
            maxRetries, retryDelayInMs);

    public Task PlayerMovedAsync(string userId, GameUpdate gameUpdate, int maxRetries = 3, int retryDelayInMs = 1000) =>
        sendWithAck(userId,
            (messageId) => hubContext.Clients.User(userId)
                .PlayerMoved(new AckMessage<GameUpdate>(messageId, gameUpdate)),
            nameof(IGameClient.PlayerMoved),
            maxRetries, retryDelayInMs);

    public Task OpponentConnectionLostAsync(string userId, int reconnectionTimeout, int maxRetries = 3,
        int retryDelayInMs = 1000) =>
        sendWithAck(userId, (messageId) => hubContext.Clients.User(userId)
                .OpponentConnectionLost(new AckMessage<int>(messageId, reconnectionTimeout)),
            nameof(IGameClient.OpponentConnectionLost),
            maxRetries, retryDelayInMs);

    public Task OpponentReconnectedAsync(string userId, int maxRetries = 3, int retryDelayInMs = 1000) =>
        sendWithAck(userId, (messageId) => hubContext.Clients.User(userId)
                .OpponentReconnected(new AckMessage(messageId)), nameof(IGameClient.OpponentReconnected),
            maxRetries, retryDelayInMs);

    public Task GameEndedAsync(string userId, GameResult gameResult, int maxRetries = 3, int retryDelayInMs = 1000) =>
        sendWithAck(userId, (messageId) => hubContext.Clients.User(userId)
                .GameEnded(new AckMessage<GameResult>(messageId, gameResult)),
            nameof(IGameClient.GameEnded),
            maxRetries, retryDelayInMs);

    public void Acknowledge(string userId, uint messageId)
    {
        if (_pendingMessagesByMessageId.TryGetValue(messageId, out PendingMessage pendingMessage))
        {
            if (pendingMessage.UserId == userId)
            {
                pendingMessage.Acknowledged = true;
            }
        }
    }

    private async Task sendWithAck(string userId, Func<uint, Task> action, string methodName, int maxRetries,
        int retryDelayInMs)
    {
        uint messageId = nextMessageId();
        var entry = _pendingMessagesByMessageId.AddOrUpdate(messageId, (_) => new PendingMessage(userId),
            (_, _) => new PendingMessage(userId));

        while (!entry.Acknowledged && entry.Attempts < maxRetries)
        {
            entry.Attempts++;

            try
            {
                await action(messageId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Send attempt failed for message {messageId} to {userId}: {ex.Message}");
            }

            await Task.Delay(retryDelayInMs);
        }

        if (!entry.Acknowledged)
            Console.WriteLine($"Sending {methodName} to {userId} failed after {entry.Attempts} attempts.");

        _pendingMessagesByMessageId.TryRemove(messageId, out _);
    }

    private uint nextMessageId()
    {
        return Interlocked.Increment(ref _messageCounter);
    }

    private record PendingMessage(string UserId)
    {
        public int Attempts { get; set; }
        public bool Acknowledged { get; set; }
    }
}