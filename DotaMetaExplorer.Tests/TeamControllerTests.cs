using DotaMetaExplorer.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaMetaExplorer.Tests
{
    public class TeamControllerTests
    {
        private static StubMessageHandler HandlerWithTeams(out List<Team> teams)
        {
            teams = new()
        {
            new() { TeamId = 5, Name = "Team Spirit", Tag = "TS", Rating = 2100,
                    Wins = 10, Losses = 3, LastMatchTime = 0 },
            new() { TeamId = 77, Name = "Gaimin Gladiators", Tag = "GG", Rating = 2050,
                    Wins = 9, Losses = 4, LastMatchTime = 0 }
        };
            var h = new StubMessageHandler();
            h.RegisterJson(Constants.teamsAddress, teams);
            h.RegisterJson("https://api.opendota.com/api/teams/5", teams[0]);
            return h;
        }

        [Fact] 
        public async Task GetTeamByName_IsCaseInsensitive()
        {
            var handler = HandlerWithTeams(out _);
            var sut = new TeamController(new HttpClient(handler));

            var actionResult = await sut.GetTeamByName("team spirit") as OkObjectResult;
            var team = Assert.IsType<Team>(actionResult!.Value!);

            Assert.Equal(5, team.TeamId);
        }

        [Fact] 
        public async Task GetTeamById_ReturnsTeam()
        {
            var handler = HandlerWithTeams(out _);
            var sut = new TeamController(new HttpClient(handler));

            var actionResult = await sut.GetTeamById(5) as OkObjectResult;
            var team = Assert.IsType<Team>(actionResult!.Value!);

            Assert.Equal("TS", team.Tag);
        }

        [Fact]
        public async Task GetAllTeams_ReturnsEveryTeam()
        {
            var handler = HandlerWithTeams(out var teams);
            var sut = new TeamController(new HttpClient(handler));

            var actionResult = await sut.GetAllTeams() as OkObjectResult;
            var list = Assert.IsType<List<Team>>(actionResult!.Value!);

            Assert.Equal(teams.Count, list.Count);
        }
    }
}
