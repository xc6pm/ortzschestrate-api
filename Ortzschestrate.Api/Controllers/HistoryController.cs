using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ortzschestrate.Api.Models;
using Ortzschestrate.Api.Security;
using DbContext = Ortzschestrate.Data.DbContext;

namespace Ortzschestrate.Api.Controllers;

[Route("/api/[controller]/[action]")]
public class HistoryController : ControllerBase
{
    public const int MaxGamesToFetchAtOnce = 50;

    [Authorize]
    [ActionName("games")]
    public async Task<List<FinishedGameVM>> GetFinishedGames(
        int count = 10,
        int page = 1,
        [FromServices] DbContext dbContext = null
    )
    {
        if (count > MaxGamesToFetchAtOnce)
            count = MaxGamesToFetchAtOnce;

        var userId = HttpContext.User.FindId();
        var finishedGames = await dbContext.FinishedGames
            .Where(g => g.Players.Any(u => u.Id == userId))
            .OrderByDescending(g => g.Started)
            .Skip((page - 1) * count)
            .Take(count)
            .Include(g => g.Players)
            .Select(g => new FinishedGameVM(g))
            .ToListAsync();

        return finishedGames;
    }

    [Authorize]
    [ActionName("game")]
    public async Task<FinishedGameVM?> GetFinishedGame(string id, [FromServices] DbContext dbContext){
        var guid = Guid.Parse(id);
        var game = await dbContext.FinishedGames.Include(g => g.Players).FirstOrDefaultAsync(g => g.Id == guid);

        return game != null ? new FinishedGameVM(game) : null;
    }
}