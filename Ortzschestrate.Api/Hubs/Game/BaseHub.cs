using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Ortzschestrate.Api.Hubs.Game;
using Ortzschestrate.Api.Models;
using Ortzschestrate.Data.Models;
using Ortzschestrate.Web3.Actions;
using GameResult = Ortzschestrate.Web3.Actions.GameResult;

namespace Ortzschestrate.Api.Hubs;

[Authorize]
public partial class GameHub(
    PlayerCache playerCache,
    StartGame startGame,
    UserManager<User> userManager,
    ResolveGame resolveGame)
    : Hub<IGameClient>
{
    private static readonly ConcurrentDictionary<string, PendingGame> _pendingGamesByCreatorId = new();
    private static readonly ConcurrentDictionary<Guid, Models.Game> _ongoingShortGames = new();
    private static readonly SemaphoreSlim _lobbySemaphore = new(1, 1);
    private const int _reconnectionTimeout = 40000;

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        Debug.WriteLine($"New client connected {Context.ConnectionId}");
        await playerCache.OnNewConnectionAsync(Context);
        var player = playerCache.GetPlayer(Context.UserIdentifier!);
        if (player.OngoingShortGame != null)
        {
            int idxOfPlayer = player.OngoingShortGame.GetPlayerIdx(Context.UserIdentifier!);

            if (player.OngoingShortGame.ConnectionTimeoutCancellations[idxOfPlayer] != null)
            {
                await player.OngoingShortGame.ConnectionTimeoutCancellations[idxOfPlayer]!.CancelAsync();
            }
        }
        else
        {
            // To send the pending games to the newly joined client.
            await Clients.Caller.LobbyUpdated(_pendingGamesByCreatorId.Values.ToList());
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Disconnected {Context.ConnectionId}");
        bool lobbyUpdated = false;
        Player? leavingPlayer = null;
        bool countdownPlayerTimeout = false;
        await _lobbySemaphore.WaitAsync();
        try
        {
            Console.WriteLine($"Remaining connections {playerCache.GetRemainingConnections(Context.UserIdentifier!)}");
            if (playerCache.GetRemainingConnections(Context.UserIdentifier!) == 1) // Last connection's closing.
            {
                Console.WriteLine("Disconnecting last connection");
                leavingPlayer = playerCache.GetPlayer(Context.UserIdentifier!);
                Console.WriteLine(leavingPlayer.OngoingShortGame);

                if (leavingPlayer.OngoingShortGame != null)
                {
                    int playerIdx = leavingPlayer.OngoingShortGame.GetPlayerIdx(Context.UserIdentifier!);
                    Console.WriteLine(leavingPlayer.OngoingShortGame.ConnectionTimeoutCancellations[playerIdx] != null
                        ? "Existing timeout"
                        : "Starting new timeout");
                    // OnDisconnect may be triggered multiple times for one instance.
                    // This prevents another timeout starting when another is ongoing.
                    bool duplicateDisconnect =
                        leavingPlayer.OngoingShortGame.ConnectionTimeoutCancellations[playerIdx] != null;
                    if (!duplicateDisconnect)
                    {
                        countdownPlayerTimeout = true;
                        leavingPlayer.OngoingShortGame.ConnectionTimeoutCancellations[playerIdx] =
                            new CancellationTokenSource();
                    }
                }
                else if (_pendingGamesByCreatorId.TryRemove(Context.UserIdentifier!, out _))
                {
                    lobbyUpdated = true;
                }
            }
        }
        finally
        {
            _lobbySemaphore.Release();

            // Closing duplicate disconnect means the same connection we're going to wait for timeout for
            // is going to be out of memory prematurely.
            // Duplicate disconnect must wait behind the timer to be treated the same as the original disconnect.
            if (!countdownPlayerTimeout)
                playerCache.OnDisconnect(Context);

            if (lobbyUpdated)
                await Clients.All.LobbyUpdated(_pendingGamesByCreatorId.Values.ToList());
        }

        Debug.WriteLine("Connection closed");
        Debug.WriteLine(exception);
        await base.OnDisconnectedAsync(exception);

        if (countdownPlayerTimeout)
        {
            Console.WriteLine("Player left game");
            var idxOfPlayer = leavingPlayer!.OngoingShortGame!.IsPlayer1(leavingPlayer.UserId) ? 0 : 1;
            int idxOfOpponent = idxOfPlayer == 0 ? 1 : 0;

            await Clients.User(leavingPlayer.OngoingShortGame.Players[idxOfOpponent].UserId)
                .OpponentConnectionLost(_reconnectionTimeout);

            try
            {
                Console.WriteLine("Waiting for timeout");
                await Task.Delay(_reconnectionTimeout,
                    leavingPlayer.OngoingShortGame.ConnectionTimeoutCancellations[idxOfPlayer]!.Token);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"Player {leavingPlayer.Name} reconnected to {leavingPlayer.OngoingShortGame.Id}");
                List<Task> tasks =
                    [Clients.User(leavingPlayer.OngoingShortGame.Players[idxOfOpponent].UserId).OpponentReconnected()];

                bool isThisPlayersTurn = leavingPlayer.OngoingShortGame.CanMove(leavingPlayer.UserId);
                if (isThisPlayersTurn && leavingPlayer.OngoingShortGame.LastMove != null)
                {
                    // Likely the leavingPlayer hasn't received opponent's move during the connection loss.
                    // They'll reject this update if they had received it before.
                    tasks.Add(Clients.User(Context.UserIdentifier!).PlayerMoved(new GameUpdate(
                        leavingPlayer.OngoingShortGame.LastMove,
                        leavingPlayer.OngoingShortGame.RemainingTimes[idxOfOpponent].TotalMilliseconds)));
                }

                await Task.WhenAll(tasks);
                return;
            }
            finally
            {
                leavingPlayer.OngoingShortGame.ConnectionTimeoutCancellations[idxOfPlayer]!.Dispose();
                leavingPlayer.OngoingShortGame.ConnectionTimeoutCancellations[idxOfPlayer] = null;
                Console.WriteLine("Clearing out the connection after reconnect");
                playerCache.OnDisconnect(Context);
            }

            Console.WriteLine("Connection timeout");

            if (leavingPlayer.OngoingShortGame.MovesMade >= 10)
            {
                leavingPlayer.OngoingShortGame.Resign(leavingPlayer);
            }
            else
            {
                leavingPlayer.OngoingShortGame.Draw();
            }

            await finalizeEndedGameAsync(leavingPlayer.OngoingShortGame);
        }
    }

    private async Task finalizeEndedGameAsync(Models.Game game)
    {
        if (game.IsWagered)
            await resolveGame.DoAsync(game.Id, findWeb3GameResult(game));

        await Clients.Users([game.Players[0].UserId, game.Players[1].UserId])
            .GameEnded(new(game.EndGame!.EndgameType.ToString(), game.EndGame.WonSide?.AsChar));
        game.Players[0].OngoingShortGame = null;
        game.Players[1].OngoingShortGame = null;
        _ongoingShortGames.TryRemove(game.Id, out _);
    }

    private GameResult findWeb3GameResult(Models.Game game)
    {
        if (game.EndGame!.WonSide == null)
        {
            return GameResult.Draw;
        }

        if (game.EndGame.WonSide == game.PlayerColors[0])
        {
            return GameResult.Player1Won;
        }

        return GameResult.Player2Won;
    }
}