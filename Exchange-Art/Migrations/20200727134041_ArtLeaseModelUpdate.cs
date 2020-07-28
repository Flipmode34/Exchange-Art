using Microsoft.EntityFrameworkCore.Migrations;

namespace Exchange_Art.Migrations
{
    public partial class ArtLeaseModelUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtLease_Art_ArtPieceId",
                table: "ArtLease");

            migrationBuilder.DropIndex(
                name: "IX_ArtLease_ArtPieceId",
                table: "ArtLease");

            migrationBuilder.DropColumn(
                name: "ArtPieceId",
                table: "ArtLease");

            migrationBuilder.AlterColumn<int>(
                name: "ArtPiece",
                table: "ArtLease",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtLease_Art_ArtPiece",
                table: "ArtLease");

            migrationBuilder.DropIndex(
                name: "IX_ArtLease_ArtPiece",
                table: "ArtLease");

            migrationBuilder.AlterColumn<string>(
                name: "ArtPiece",
                table: "ArtLease",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "ArtPieceId",
                table: "ArtLease",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArtLease_ArtPieceId",
                table: "ArtLease",
                column: "ArtPieceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtLease_Art_ArtPieceId",
                table: "ArtLease",
                column: "ArtPieceId",
                principalTable: "Art",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
