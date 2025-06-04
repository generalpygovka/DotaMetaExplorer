using Microsoft.AspNetCore.Mvc;
using DotaMetaExplorer.Models;

namespace DotaMetaExplorer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeamController : ControllerBase
{
    readonly string _address;
    readonly HttpClient _httpclient;
    public TeamController(HttpClient httpClient)
    {
        _address = Constants.teams;
        _httpclient = httpClient;
    }

    [HttpGet("GetTeamByName")]
    public async Task<IActionResult> GetTeamByName(string name)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_address + Constants.tokenApi),
        };
        using (var response = await _httpclient.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadFromJsonAsync<List<Team>>();
            return Ok(body?.FirstOrDefault(x =>x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
        }
    }

    [HttpGet("GetTeamById")]
    public async Task<IActionResult> GetTeamById(int id)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(Constants.teams + $"/{id}" + Constants.tokenApi),
        };
        using (var response = await _httpclient.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadFromJsonAsync<Team>();
            return Ok(body);
        }
    }

    [HttpGet("GetAllTeams")]
    public async Task<IActionResult> GetAllTeams()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_address + Constants.tokenApi),
        };
        using (var response = await _httpclient.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var bodys = await response.Content.ReadFromJsonAsync<List<Team>>();
            return Ok(bodys);
        }
    }
}


