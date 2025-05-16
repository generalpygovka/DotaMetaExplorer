using DotaMetaExplorer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DotaMetaExplorer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        readonly string _address;
        public MatchController()
        {
            _address = Constants.proMatches;
        }
        [HttpGet("GetRecentMatches")]

        public async Task<IActionResult> GetRecentMatches()
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
                var body = await response.Content.ReadFromJsonAsync<List<ProMatches>>();
                var last10 = body?.Take(10).Select(x => new {x.MatchId,
                Teams = $"{x.RadiantName} vs {x.DireName}",
                Winner = x.RadiantWin ? x.RadiantName : x.DireName,
                Duration = TimeSpan.FromSeconds(x.Duration).ToString(@"hh\:mm\:ss")});
                return Ok(last10);
            }
        }
    }
}
