using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NBitcoin.Secp256k1;
using Ortzschestrate.Api.Models;
using Ortzschestrate.Api.Security;
using Ortzschestrate.Data.Models;
using DbContext = Ortzschestrate.Data.DbContext;
using FinishedGame = Ortzschestrate.Data.Models.FinishedGame;

namespace Ortzschestrate.Api.Controllers;

[Route("/api/[controller]/[action]")]
public class HistoryController : ControllerBase
{
    public const int MaxGamesToFetchAtOnce = 50;

    [Authorize]
    [ActionName("games")]
    public async Task<List<Api.Models.FinishedGame>> GetFinishedGames(
        int count = 10,
        int page = 1,
        [FromServices] DbContext dbContext = null
    )
    {
        if (count > MaxGamesToFetchAtOnce)
            count = MaxGamesToFetchAtOnce;

        var userId = HttpContext.User.FindId();
        var finishedGames = await dbContext
            .FinishedGames.Where(g => g.Players.Any(u => u.Id == userId))
            .OrderBy(g => g.Started)
            .Skip((page - 1) * count)
            .Take(count)
            .Select(g => new Models.FinishedGame(
                g.Id.ToString(),
                g.Players.Select(p => new Player(p.Id, p.UserName!)).ToArray(),
                g.PlayerColors.Select(c => c == Color.White ? 'w' : 'b').ToArray(),
                g.StakeEth,
                g.TimeInMs,
                g.Started,
                g.RemainingTimesInMs.ToArray(),
                g.Pgn,
                g.EndGameType.ToString(),
                g.WonSide == null ? null
                    : g.WonSide == Color.White ? 'w'
                    : 'b'
            ))
            .ToListAsync();

        return finishedGames;
    }
}
