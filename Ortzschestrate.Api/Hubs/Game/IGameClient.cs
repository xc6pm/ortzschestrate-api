using Ortzschestrate.Api.Models;

namespace Ortzschestrate.Api.Hubs;

public interface IGameClient
{
    Task LobbyUpdated(List<PendingGame> pendingGames);
    Task GameStarted(AckMessage<string> gameId);
    Task PlayerMoved(AckMessage<GameUpdate> gameUpdate);
    Task OpponentConnectionLost(AckMessage<int> reconnectionTimeout);
    Task OpponentReconnected(AckMessage ack);
    Task GameEnded(AckMessage<GameResult> gameResult);
    Task GameHistoryUpdated(AckMessage<FinishedGameVM> addedGame);
}
