using DotaMetaExplorer.Context;
using DotaMetaExplorer.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace DotaMetaExplorer.Services;
public class LeaderboardCacheService
{
    private readonly ApplicationDBContext _context;
    

    public LeaderboardCacheService(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<bool> IsCacheActualAsync(TimeSpan cacheLifetime)
    {
        var cache = await _context.RatingCaches.FirstOrDefaultAsync();
        if (cache == null) return false;
        return (DateTime.UtcNow - cache.LastCacheDateTime) < cacheLifetime;
    }

    public async Task UpdateLeaderboardCacheAsync()
    {
        var client = new HttpClient();
        var proPlayers = await client.GetFromJsonAsync<List<ProPlayer>>(Constants.proPlayers);

        var leaderboard = new List<PlayerRankCache>();

        foreach (var proPlayer in proPlayers!)
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
                Rank = player.LeaderboardRank.Value
            });
        }

        _context.PlayerRanksCache.RemoveRange(_context.PlayerRanksCache);
        await _context.PlayerRanksCache.AddRangeAsync(leaderboard.OrderBy(x => x.Rank).Take(10));

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
