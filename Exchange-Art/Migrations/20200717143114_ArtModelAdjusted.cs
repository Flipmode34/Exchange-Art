using Microsoft.EntityFrameworkCore.Migrations;

namespace Exchange_Art.Migrations
{
    public partial class ArtModelAdjusted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Art_AspNetUsers_Owner",
                table: "Art");

            migrationBuilder.RenameColumn(
                name: "Owner",
                table: "Art",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Art_Owner",
                table: "Art",
                newName: "IX_Art_OwnerId");

            migrationBuilder.AddColumn<string>(
                name: "OwnerName",
                table: "Art",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Art_AspNetUsers_OwnerId",
                table: "Art",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Art_AspNetUsers_OwnerId",
                table: "Art");

            migrationBuilder.DropColumn(
                name: "OwnerName",
                table: "Art");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Art",
                newName: "Owner");

            migrationBuilder.RenameIndex(
                name: "IX_Art_OwnerId",
                table: "Art",
                newName: "IX_Art_Owner");

            migrationBuilder.AddForeignKey(
                name: "FK_Art_AspNetUsers_Owner",
                table: "Art",
                column: "Owner",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
