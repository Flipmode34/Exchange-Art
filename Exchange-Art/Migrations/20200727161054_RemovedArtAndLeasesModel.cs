using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Exchange_Art.Migrations
{
    public partial class RemovedArtAndLeasesModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArtDescription",
                table: "ArtLease",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ArtPieceImageData",
                table: "ArtLease",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArtDescription",
                table: "ArtLease");

            migrationBuilder.DropColumn(
                name: "ArtPieceImageData",
                table: "ArtLease");
        }
    }
}
