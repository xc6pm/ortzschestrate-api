using Ortzschestrate.Api.Models;

namespace Ortzschestrate.Api.Hubs;

public interface IGameClient
{
    Task LobbyUpdated(List<PendingGame> pendingGames);
    Task GameStarted(string gameId);
    Task PlayerMoved(GameUpdate gameUpdate);
    Task GameEnded(GameResult gameResult);    
}