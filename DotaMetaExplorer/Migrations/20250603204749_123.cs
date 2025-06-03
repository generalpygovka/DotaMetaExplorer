using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotaMetaExplorer.Migrations
{
    /// <inheritdoc />
    public partial class _123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rank",
                table: "PlayerRanksCache",
                newName: "LeaderboardRank");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LeaderboardRank",
                table: "PlayerRanksCache",
                newName: "Rank");
        }
    }
}
