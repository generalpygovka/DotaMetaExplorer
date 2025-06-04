using DotaMetaExplorer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotaMetaExplorer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GifController : ControllerBase
{
    [HttpGet("GetByTag")]
    public async Task<IActionResult> GetByTag(string tag)
    {

        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://api.giphy.com/v1/gifs/random?api_key=m5oqHqYo9sv1Wm58U178qUgvNw4GucTD&tag="+ tag + "&rating=g"),
        };
        using (var response = await client.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadFromJsonAsync<Giphy.RandomGiphy>();
            return Ok(body);
        }
    }
}
