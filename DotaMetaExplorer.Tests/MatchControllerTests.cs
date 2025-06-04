using DotaMetaExplorer.Controllers;
using DotaMetaExplorer.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaMetaExplorer.Tests
{
    public class MatchControllerTests
    {
        [Fact] 
        public async Task GetRecentMatches_ReturnsLast10()
        {
            var handler = new StubMessageHandler();
            var sample = Enumerable.Range(1, 15).Select(i => new ProMatches
            {
                MatchId = i,
                Duration = 100,
                RadiantWin = i % 2 == 0
            }).ToList();
            handler.RegisterJson(Constants.proMatches + "?api_key=76b37873-3339-4684-a85b-d67c7605a573", sample);

            var sut = new MatchController(new HttpClient(handler));
            var actionResult = await sut.GetRecentMatches() as OkObjectResult;
            var list = Assert.IsType<List<ProMatches>>(actionResult!.Value!);

            Assert.Equal(10, list.Count);
            for (int i = 0; i < 10; i++)
                Assert.Equal(sample[i].MatchId, list[i].MatchId);
        }
    }
}
