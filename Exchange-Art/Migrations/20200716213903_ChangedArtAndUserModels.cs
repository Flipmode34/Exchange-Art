using Microsoft.EntityFrameworkCore.Migrations;

namespace Exchange_Art.Migrations
{
    public partial class ChangedArtAndUserModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageTitle",
                table: "Art",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageDescription",
                table: "Art",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Owner",
                table: "Art",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Art_Owner",
                table: "Art",
                column: "Owner");

            migrationBuilder.AddForeignKey(
                name: "FK_Art_AspNetUsers_Owner",
                table: "Art",
                column: "Owner",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Art_AspNetUsers_Owner",
                table: "Art");

            migrationBuilder.DropIndex(
                name: "IX_Art_Owner",
                table: "Art");

            migrationBuilder.DropColumn(
                name: "ImageDescription",
                table: "Art");

            migrationBuilder.DropColumn(
                name: "Owner",
                table: "Art");

            migrationBuilder.AlterColumn<string>(
                name: "ImageTitle",
                table: "Art",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
