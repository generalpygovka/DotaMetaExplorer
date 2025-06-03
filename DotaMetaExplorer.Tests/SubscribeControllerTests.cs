using DotaMetaExplorer.Controllers;
using DotaMetaExplorer.Context;
using DotaMetaExplorer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

namespace DotaMetaExplorer.Tests;
[Collection("Sequential")]
public class SubscribeControllerTests
{
    private ApplicationDBContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDBContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Subscribe")
            .Options;
        return new ApplicationDBContext(options);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult()
    {
        var context = GetDbContext();
        var controller = new SubscribeController(context);
        var result = await controller.GetAll();
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Save_ReturnsOkResult()
    {
        var context = GetDbContext();
        var controller = new SubscribeController(context);
        var subscribe = new Subscribe { ChatId = 1 };
        var result = await controller.Save(subscribe);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsOkResult()
    {
        var context = GetDbContext();
        context.Subscribes.Add(new Subscribe { ChatId = 2 });
        context.SaveChanges();
        var controller = new SubscribeController(context);
        var result = await controller.Delete(2);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult()
    {
        var context = GetDbContext();
        context.Subscribes.Add(new Subscribe { ChatId = 3 });
        context.SaveChanges();
        var controller = new SubscribeController(context);
        var result = await controller.GetById(3);
        Assert.IsType<OkObjectResult>(result);
    }
}
