using Microsoft.EntityFrameworkCore.Migrations;

namespace DreamSoccer.Repository.Migrations
{
    public partial class ChangeMappingPlayerAndTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Players",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TeamId",
                table: "Users",
                column: "TeamId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Teams_TeamId",
                table: "Users",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Teams_TeamId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_TeamId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Players");
        }
    }
}
