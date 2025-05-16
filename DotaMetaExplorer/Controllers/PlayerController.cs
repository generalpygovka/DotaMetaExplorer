using Azure.Core;
using Azure;
using DotaMetaExplorer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DotaMetaExplorer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        readonly string _address;
        public PlayerController()
        {
            _address = Constants.proPlayers;
        }

        [HttpGet("GetProPlayers")]
        public async Task<IActionResult> GetProPlayers()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_address),
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadFromJsonAsync<List<ProPlayer>>();
                return Ok(body);
            }
        }

        [HttpGet("GetPlayerById")]
        public async Task<IActionResult> GetPlayers(int id)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api.opendota.com/api/players/{id}"),
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadFromJsonAsync<Player>();
                return Ok(body);
            }
        }

        [HttpGet("GetLeaderboards")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_address),
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var proPlayers = await response.Content.ReadFromJsonAsync<List<ProPlayer>>();

                var leaderboard = new List<(ProPlayer Player, int? LeaderboardRank)>();

                foreach (var proPlayer in proPlayers!)
                {
                    var playerResponse = await client.GetAsync($"https://api.opendota.com/api/players/{proPlayer.AccountId}");
                    var player = await playerResponse.Content.ReadFromJsonAsync<Player>();
                    leaderboard.Add((proPlayer, player?.LeaderboardRank));
                }

                var top = leaderboard.Where(x => x.LeaderboardRank.HasValue).OrderBy(x => x.LeaderboardRank).Take(10)
                    .Select(x => new
                    {
                        x.Player.PersonaName,
                        x.Player.AccountId,
                        x.LeaderboardRank
                    }).ToList();
                return Ok(top);
            }
        }
    }
}
