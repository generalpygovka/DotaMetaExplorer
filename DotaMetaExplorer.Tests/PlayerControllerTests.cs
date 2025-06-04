using DotaMetaExplorer.Context;
using DotaMetaExplorer.Controllers;
using DotaMetaExplorer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotaMetaExplorer.Tests;

public class PlayerControllerTests
{
    private static (PlayerController Sut, ApplicationDBContext Ctx) CreateDbAndController(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                          .UseInMemoryDatabase(dbName)
                          .Options;
        var ctx = new ApplicationDBContext(options);
        var handler = new StubMessageHandler();
        var client = new HttpClient(handler);
        return (new PlayerController(ctx, client), ctx);
    }

    [Fact]  
    public async Task GetLeaderboardDatabase_ReturnsOrderedRanks()
    {
        var (sut, ctx) = CreateDbAndController(nameof(GetLeaderboardDatabase_ReturnsOrderedRanks));
        ctx.PlayerRanksCache.AddRange(
            new PlayerRankCache { LeaderboardRank = 5, AccountId = 50, PersonaName = "X" },
            new PlayerRankCache { LeaderboardRank = 1, AccountId = 10, PersonaName = "Y" }
        );
        await ctx.SaveChangesAsync();

        var actionResult = await sut.GetLeaderboardDatabase() as OkObjectResult;
        var list = Assert.IsType<List<PlayerRankCache>>(actionResult!.Value!);

        Assert.Equal(new[] { 1, 5 }, list.Select(r => r.LeaderboardRank).ToArray());
    }
    [Fact] 
    public async Task GetPlayers_ReturnsPlayer()
    {
        var handler = new StubMessageHandler();
        handler.RegisterJson(Constants.players + "/99" + Constants.tokenApi,
                             new Player { RankTier = 7 });
        var (sut, _) = CreateDbAndController(nameof(GetPlayers_ReturnsPlayer));
        typeof(PlayerController)
            .GetField("_httpclient", BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(sut, new HttpClient(handler));

        var actionResult = await sut.GetPlayers(99) as OkObjectResult;
        var player = Assert.IsType<Player>(actionResult!.Value!);

        Assert.Equal(7, player.RankTier);
    }

}
