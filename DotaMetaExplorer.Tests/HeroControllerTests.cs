using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using DotaMetaExplorer.Controllers;
using DotaMetaExplorer.Models;
namespace DotaMetaExplorer.Tests;

public sealed class StubMessageHandler : HttpMessageHandler
{
    private readonly Dictionary<string, HttpResponseMessage> _map = new();

    public void RegisterJson<T>(string uri, T payload,
                                HttpStatusCode code = HttpStatusCode.OK)
    {
        var json = JsonSerializer.Serialize(payload,
                   new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        _map[uri] = new HttpResponseMessage(code)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage req,
                                                           CancellationToken token)
    {
        if (_map.TryGetValue(req.RequestUri!.ToString(), out var resp))
            return Task.FromResult(resp);
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
    }
}
public class HeroControllerTests
{
    private static (HttpClient client, List<Hero> heroes) HeroClient()
    {
        var heroes = new List<Hero>
        {
            new() { Id = 1, Name = "axe", LocalizedName = "Axe", PrimaryAttr = "str",
                    AttackType = "Melee", Roles = new[] { "Initiator" } },
            new() { Id = 2, Name = "drow_ranger", LocalizedName = "Drow Ranger",
                    PrimaryAttr = "agi", AttackType = "Ranged", Roles = new[] { "Carry" } }
        };
        var h = new StubMessageHandler();
        h.RegisterJson(Constants.heroesAddress, heroes);
        return (new HttpClient(h), heroes);
    }

    [Fact] 
    public async Task GetByName_ReturnsCaseInsensitiveMatch()
    {
        var (client, _) = HeroClient();
        var sut = new HeroController(client);

        var actionResult = await sut.GetByName("axE") as OkObjectResult;
        var hero = Assert.IsType<Hero>(actionResult!.Value!);

        Assert.Equal(1, hero.Id);
    }

    [Fact] 
    public async Task GetAllHeroes_ReturnsFullList()
    {
        var (client, heroes) = HeroClient();
        var sut = new HeroController(client);

        var actionResult = await sut.GetAllHeroes() as OkObjectResult;
        var list = Assert.IsType<List<Hero>>(actionResult!.Value!);

        Assert.Equal(heroes.Count, list.Count);
    }

    [Fact]
    public async Task GetByIdHero_ReturnsCorrectHero()
    {
        var (client, _) = HeroClient();
        var sut = new HeroController(client);

        var actionResult = await sut.GetByIdHero(2) as OkObjectResult;
        var hero = Assert.IsType<Hero>(actionResult!.Value!);

        Assert.Equal("Drow Ranger", hero.LocalizedName);
    }

    [Fact] 
    public async Task GetRandomHero_ReturnsHeroFromSet()
    {
        var (client, heroes) = HeroClient();
        var sut = new HeroController(client);

        var actionResult = await sut.GetRandomHero() as OkObjectResult;
        var hero = Assert.IsType<Hero>(actionResult!.Value!);

        Assert.Contains(heroes, h => h.Id == hero.Id);
    }
}

