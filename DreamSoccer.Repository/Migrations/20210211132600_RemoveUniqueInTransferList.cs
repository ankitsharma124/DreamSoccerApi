using Microsoft.EntityFrameworkCore.Migrations;

namespace DreamSoccer.Repository.Migrations
{
    public partial class RemoveUniqueInTransferList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferList_Players_PlayerId",
                table: "TransferList");

            migrationBuilder.DropIndex(
                name: "IX_TransferList_PlayerId",
                table: "TransferList");

            migrationBuilder.CreateIndex(
                name: "IX_TransferList_PlayerId",
                table: "TransferList",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_TransferListId",
                table: "Players",
                column: "TransferListId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferList_Players_PlayerId",
                table: "TransferList",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferList_Players_PlayerId",
                table: "TransferList");

            migrationBuilder.DropIndex(
                name: "IX_TransferList_PlayerId",
                table: "TransferList");

            migrationBuilder.DropIndex(
                name: "IX_Players_TransferListId",
                table: "Players");

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
    }
}
