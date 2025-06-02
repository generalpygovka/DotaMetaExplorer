using DotaMetaExplorer.Context;
using DotaMetaExplorer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotaMetaExplorer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubscribeController : ControllerBase
{
    private readonly ApplicationDBContext context;
    public SubscribeController(ApplicationDBContext context)
    {
        this.context = context;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var all = await context.Subscribes.ToListAsync();
        return Ok(all);
    }

    [HttpPost("Subscribe")]
    public async Task<IActionResult> Save(Subscribe subscribe)
    {
        context.Subscribes.Add(subscribe);
        await context.SaveChangesAsync();
        return Ok(subscribe);
    }

    [HttpDelete("DeleteSubscribe")]
    public async Task<IActionResult> Delete(int id)
    {
        var subscribeToDelete = await context.Subscribes.FirstOrDefaultAsync(x => x.ChatId == id);
        context.Subscribes.Remove(subscribeToDelete!);
        await context.SaveChangesAsync();
        return Ok(subscribeToDelete);
    }

    [HttpGet("GetById")]
    public async Task<IActionResult> GetById(int id)
    {
        var subscribes = await context.Subscribes.Where(x => x.ChatId == id).ToListAsync();
        return Ok(subscribes);
    }
}
