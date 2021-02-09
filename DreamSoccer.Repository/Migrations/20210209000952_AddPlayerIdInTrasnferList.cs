using Microsoft.EntityFrameworkCore.Migrations;

namespace DreamSoccer.Repository.Migrations
{
    public partial class AddPlayerIdInTrasnferList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_TransferList_TransferListId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_TransferListId",
                table: "Players");

            migrationBuilder.AddColumn<int>(
                name: "PlayerId",
                table: "TransferList",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TransferList_PlayerId",
                table: "TransferList",
                column: "PlayerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferList_Players_PlayerId",
                table: "TransferList",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferList_Players_PlayerId",
                table: "TransferList");

            migrationBuilder.DropIndex(
                name: "IX_TransferList_PlayerId",
                table: "TransferList");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "TransferList");

            migrationBuilder.CreateIndex(
                name: "IX_Players_TransferListId",
                table: "Players",
                column: "TransferListId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_TransferList_TransferListId",
                table: "Players",
                column: "TransferListId",
                principalTable: "TransferList",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
