using Microsoft.AspNetCore.Mvc;
using DotaMetaExplorer.Models;

namespace DotaMetaExplorer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeamController : ControllerBase
{
    readonly string _address;
    public TeamController()
    {
        _address = Constants.teamsAddress;
    }

    [HttpGet("GetTeamByName")]
    public async Task<IActionResult> GetTeam(string name)
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
            var body = await response.Content.ReadFromJsonAsync<List<Team>>();
            return Ok(body?.FirstOrDefault(x =>x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
        }
    }

    [HttpGet("GetAllTeams")]
    public async Task<IActionResult> GetAllTeams()
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
            var bodys = await response.Content.ReadFromJsonAsync<List<Team>>();
            return Ok(bodys);
        }
    }
}


