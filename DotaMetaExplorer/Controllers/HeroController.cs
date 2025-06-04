using Microsoft.AspNetCore.Mvc;
using DotaMetaExplorer.Models;
using System;

namespace DotaMetaExplorer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HeroController : ControllerBase
{
    readonly string _address;
    readonly HttpClient _httpclient;
    public HeroController(HttpClient httpClient)
    {
        _address = Constants.heroesAddress;
        _httpclient = httpClient;
    }

    [HttpGet("GetHeroByName")]
    public async Task<IActionResult> GetByName(string name)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_address),
        };
        using (var response = await _httpclient.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadFromJsonAsync<List<Hero>>();
            return Ok(body?.FirstOrDefault(x => x.LocalizedName.Equals(name, StringComparison.OrdinalIgnoreCase)));
        }
    }

    [HttpGet("GetAllHeroes")]
    public async Task<IActionResult> GetAllHeroes()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_address + "?api_key=76b37873-3339-4684-a85b-d67c7605a573"),
        };
        using (var response = await _httpclient.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadFromJsonAsync<List<Hero>>();
            return Ok(body);
        }
    }

    [HttpGet("GetHeroById")]
    public async Task<IActionResult> GetByIdHero(int id)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_address + "?api_key=76b37873-3339-4684-a85b-d67c7605a573"),
        };
        using (var response = await _httpclient.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadFromJsonAsync<List<Hero>>();
            return Ok(body?.FirstOrDefault(x => x.Id == id));
        }
    }

    [HttpGet("GetRandomHero")]
    public async Task<IActionResult> GetRandomHero()
    {
        var random = new Random();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_address + "?api_key=76b37873-3339-4684-a85b-d67c7605a573"),
        };
        using (var response = await _httpclient.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadFromJsonAsync<List<Hero>>();
            int randomNumber = random.Next(body!.Count);
            Hero chosen = body[randomNumber];
            return Ok(body.FirstOrDefault(x => x.Id == chosen.Id));
        }
    }
}
