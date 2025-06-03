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
    public class PatchControllerTests
    {
        private static StubMessageHandler HandlerForPatches()
        {
            var h = new StubMessageHandler();
            var patchList = new PatchList
            {
                Success = true,
                Patches = new List<PatchInfo>
            {
                new() { PatchNumber = "7.36" },
                new() { PatchNumber = "7.37" }
            }
            };
            // первый запрос: список патчей
            h.RegisterJson("https://www.dota2.com/datafeed/patchnoteslist?language=Ukrainian",
                           patchList);
            // второй запрос: конкретные notes для версии 7.37
            h.RegisterJson(
                "https://www.dota2.com/datafeed/patchnotes?language=Ukrainian&version=7.37",
                new PatchNotes
                {
                    PatchNumber = "7.37",
                    PatchName = "TestPatch",
                    PatchTimestamp = 0,
                    Success = true
                }
            );
            return h;
        }
        [Fact]  /* GetLatestPatch (Latest) */
        public async Task Latest_EndpointsGivesJson()
        {
            var handler = HandlerForPatches();
            var sut = new PatchController(new HttpClient(handler));

            var actionResult = await sut.Latest() as OkObjectResult;
            // actionResult.Value – это анонимный объект { latest_patch = "7.37" }
            var anon = actionResult!.Value!;
            var prop = anon.GetType().GetProperty("latest_patch");
            Assert.NotNull(prop);

            var patch = prop.GetValue(anon) as string;
            Assert.Equal("7.37", patch);
        }

        [Fact]  /* LatestNotes */
        public async Task LatestNotes_ReturnsNotesForLatest()
        {
            var handler = HandlerForPatches();
            var sut = new PatchController(new HttpClient(handler));

            var actionResult = await sut.LatestNotes() as OkObjectResult;
            var notes = Assert.IsType<PatchNotes>(actionResult!.Value!);

            Assert.Equal("7.37", notes.PatchNumber);
        }
    }
}
