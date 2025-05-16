using DotaMetaExplorer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DotaMetaExplorer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatchController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string ListUrl = "https://www.dota2.com/datafeed/patchnoteslist?language=Russian";
    private readonly string NotesUrl = "https://www.dota2.com/datafeed/patchnotes?language=Russian&version={0}";

    public PatchController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private async Task<string> GetLatestVersionAsync()
    {
        var client = _httpClientFactory.CreateClient();
        var json = await client.GetStringAsync(ListUrl);
        var list = JsonSerializer.Deserialize<PatchList>(json);
        return list.Patches.Last().PatchNumber;
    }

    [HttpGet("latest")]
    public async Task<IActionResult> Latest()
    {
        var version = await GetLatestVersionAsync();
        return Ok(new { latest_patch = version });
    }

    [HttpGet("latest/notes")]
    public async Task<IActionResult> LatestNotes()
    {
        var version = await GetLatestVersionAsync();
        var client = _httpClientFactory.CreateClient();
        var json = await client.GetStringAsync(string.Format(NotesUrl, version));
        var notes = JsonSerializer.Deserialize<PatchNotesDto>(json);
        return Ok(notes);
    }

}
