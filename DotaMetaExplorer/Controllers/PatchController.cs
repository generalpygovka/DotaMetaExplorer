using DotaMetaExplorer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace DotaMetaExplorer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatchController : ControllerBase
{
    readonly HttpClient _httpclient;
    public PatchController(HttpClient httpClient)
    {
        _httpclient = httpClient;
    }
    public async Task<string> GetLatestVersionAsync()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://www.dota2.com/datafeed/patchnoteslist?language=Ukrainian"),
        };
        using (var response = await _httpclient.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadFromJsonAsync<PatchList>();
            return body!.Patches!.Last().PatchNumber!;
        }
    }

    [HttpGet("GetLatestPatch")]
    public async Task<IActionResult> Latest()
    {
        var version = await GetLatestVersionAsync();
        return Ok(new { latest_patch = version });
    }

    [HttpGet("GetLatestNotes")]
    public async Task<IActionResult> LatestNotes()
    {
        var client = new HttpClient();
        var version = await GetLatestVersionAsync();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://www.dota2.com/datafeed/patchnotes?language=Ukrainian&version=" + version),
        };
        using (var response = await _httpclient.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadFromJsonAsync<PatchNotes>();
            return Ok(body);
        }
    }

}
