using Microsoft.AspNetCore.Mvc;
using DotaMetaExplorer.Models;
using System;

namespace DotaMetaExplorer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeroController : ControllerBase
    {
        readonly string _address;
        public HeroController()
        {
            _address = Constants.heroesAddress;
        }

        [HttpGet("GetByName")]
        
        public async Task<IActionResult> GetByName(string name)
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
                var body = await response.Content.ReadFromJsonAsync<List<Hero>>();
                return Ok(body?.FirstOrDefault(x => x.LocalizedName.Equals(name, StringComparison.OrdinalIgnoreCase)));
            }
        }

        [HttpGet("GetAllHeroes")]
        
        public async Task<IActionResult> GetAllHeroes()
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
                var body = await response.Content.ReadFromJsonAsync<List<Hero>>();
                return Ok(body);
            }
        }

        [HttpGet("GetByIdHero")]
        public async Task<IActionResult> GetByIdHero(int id)
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
                var body = await response.Content.ReadFromJsonAsync<List<Hero>>();
                return Ok(body?.FirstOrDefault(x => x.Id == id));
            }
        }

        [HttpGet("GetRandomHero")]
        public async Task<IActionResult> GetRandomHero()
        {
            var random = new Random();
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_address),
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadFromJsonAsync<List<Hero>>();
                int randomNumber = random.Next(body!.Count);
                Hero chosen = body[randomNumber];
                return Ok(body.FirstOrDefault(x => x.Id == chosen.Id));
            }
        }
    }
}
