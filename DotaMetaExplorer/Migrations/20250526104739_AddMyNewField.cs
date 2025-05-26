using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotaMetaExplorer.Migrations
{
    /// <inheritdoc />
    public partial class AddMyNewField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSubscribeForPatch",
                table: "Subscribes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSubscribeForPatch",
                table: "Subscribes");
        }
    }
}
