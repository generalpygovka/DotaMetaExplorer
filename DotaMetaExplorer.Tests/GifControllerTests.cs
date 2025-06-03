using DotaMetaExplorer.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using System.Threading.Tasks;

namespace DotaMetaExplorer.Tests;
[Collection("Sequential")]
public class GifControllerTests
{
    [Fact]
    public async Task GetByTag_ReturnsOkResult()
    {
        var controller = new GifController();
        var result = await controller.GetByTag("dota");
        Assert.IsType<OkObjectResult>(result);
    }
}
