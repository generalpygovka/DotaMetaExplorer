using DotaMetaExplorer.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotaMetaExplorer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayerController : ControllerBase
{
    private readonly ApplicationDBContext _context;
    readonly HttpClient _httpclient;
    public PlayerController(ApplicationDBContext context, HttpClient httpclient)
    {
        _context = context;
        _httpclient = httpclient;
    }

    [HttpGet("GetLeaderBoardDatabase")]
    public async Task<IActionResult> GetLeaderboardDatabase()
    {
        var leaderboard = await _context.PlayerRanksCache.OrderBy(x => x.LeaderboardRank).ToListAsync();
        return Ok(leaderboard);
    }

    [HttpGet("GetProPlayers")]
    public async Task<IActionResult> GetProPlayers()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(Constants.proPlayers),
        };
        using (var response = await _httpclient.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadFromJsonAsync<List<ProPlayer>>();
            return Ok(body);
        }
    }

    [HttpGet("GetPlayerById")]
    public async Task<IActionResult> GetPlayers(int id)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"https://api.opendota.com/api/players/{id}"),
        };
        using (var response = await _httpclient.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadFromJsonAsync<Player>();
            return Ok(body);
        }
    }

    [HttpGet("GetLeaderboardSlow")]
    public async Task<IActionResult> GetLeaderboardSlow()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(Constants.proPlayers),
        };
        using (var response = await _httpclient.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var proPlayers = await response.Content.ReadFromJsonAsync<List<ProPlayer>>();

            var leaderboard = new List<(ProPlayer Player, int? LeaderboardRank)>();

            foreach (var proPlayer in proPlayers!)
            {
                if (proPlayer.AccountId == null)
                    continue; // Пропустить игроков без профиля
                var playerResponse = await _httpclient.GetAsync($"https://api.opendota.com/api/players/{proPlayer.AccountId}");
                var player = await playerResponse.Content.ReadFromJsonAsync<Player>();
                leaderboard.Add((proPlayer, player?.LeaderboardRank));
            }

            var top = leaderboard.Where(x => x.LeaderboardRank.HasValue).OrderBy(x => x.LeaderboardRank).Take(10).Select(x => new
            {
                x.Player.PersonaName,
                x.Player.AccountId,
                x.LeaderboardRank
            }).ToList();
            return Ok(top);
        }
    }
}
