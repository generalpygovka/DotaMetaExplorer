using DotaMetaExplorer.Context;
using DotaMetaExplorer.Models;
using Microsoft.EntityFrameworkCore;

namespace DotaMetaExplorer.Services;

public class LeaderboardCacheService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public LeaderboardCacheService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
        
  
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await UpdateLeaderboardCacheAsync();
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            await UpdateLeaderboardCacheAsync();
        }
    }

    public async Task UpdateLeaderboardCacheAsync()
    {
        var client = new HttpClient();
        using var scope = _scopeFactory.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

        var proPlayers = await client.GetFromJsonAsync<List<ProPlayer>>(Constants.proPlayers );

        var leaderboard = new List<PlayerRankCache>();

        if (proPlayers != null)
        {
            foreach (var proPlayer in proPlayers)
            {
                if (proPlayer.AccountId == null)
                    continue;

                var playerResponse = await client.GetAsync($"https://api.opendota.com/api/players/{proPlayer.AccountId}");
                if (!playerResponse.IsSuccessStatusCode)
                    continue;

                var player = await playerResponse.Content.ReadFromJsonAsync<Player>();
                if (player?.LeaderboardRank == null)
                    continue;

                leaderboard.Add(new PlayerRankCache
                {
                    AccountId = proPlayer.AccountId.Value,
                    PersonaName = proPlayer.PersonaName ?? "",
                    LeaderboardRank = player.LeaderboardRank.Value
                });
            }
        }

        _context.PlayerRanksCache.RemoveRange(_context.PlayerRanksCache);
        await _context.PlayerRanksCache.AddRangeAsync(leaderboard.OrderBy(x => x.LeaderboardRank).Take(10));

        var cache = await _context.RatingCaches.FirstOrDefaultAsync();
        if (cache == null)
        {
            cache = new RatingCache { LastCacheDateTime = DateTime.UtcNow };
            _context.RatingCaches.Add(cache);
        }
        else
        {
            cache.LastCacheDateTime = DateTime.UtcNow;
            _context.RatingCaches.Update(cache);
        }

        await _context.SaveChangesAsync();
    }
}
