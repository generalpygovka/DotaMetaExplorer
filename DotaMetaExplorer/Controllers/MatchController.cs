using DotaMetaExplorer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DotaMetaExplorer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MatchController : ControllerBase
{
    readonly string _address;
    readonly HttpClient _httpclient;
    public MatchController(HttpClient httpClient)
    {
        _address = Constants.proMatches;
        _httpclient = httpClient;
    }
    [HttpGet("GetRecentMatches")]

    public async Task<IActionResult> GetRecentMatches()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_address),
        };
        using (var response = await _httpclient.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadFromJsonAsync<List<ProMatches>>();
            var last10 = body?.Take(10).ToList();
            return Ok(last10);
        }
    }
}
