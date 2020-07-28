using Microsoft.EntityFrameworkCore.Migrations;

namespace Exchange_Art.Migrations
{
    public partial class ArtLeaseModelUpdate3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtLease_Art_ArtPiece",
                table: "ArtLease");

            migrationBuilder.DropIndex(
                name: "IX_ArtLease_ArtPiece",
                table: "ArtLease");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ArtLease_ArtPiece",
                table: "ArtLease",
                column: "ArtPiece");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtLease_Art_ArtPiece",
                table: "ArtLease",
                column: "ArtPiece",
                principalTable: "Art",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
