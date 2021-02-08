using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DreamSoccer.Repository.Migrations
{
    public partial class CreateTransferList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransferListId",
                table: "Players",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TransferList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdateAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DelFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferList", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_TransferList_TransferListId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "TransferList");

            migrationBuilder.DropIndex(
                name: "IX_Players_TransferListId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "TransferListId",
                table: "Players");
        }
    }
}
