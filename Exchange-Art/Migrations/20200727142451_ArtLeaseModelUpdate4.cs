using Microsoft.EntityFrameworkCore.Migrations;

namespace Exchange_Art.Migrations
{
    public partial class ArtLeaseModelUpdate4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtLease_AspNetUsers_LeaseUserId",
                table: "ArtLease");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtLease_AspNetUsers_OwnerUserId",
                table: "ArtLease");

            migrationBuilder.DropIndex(
                name: "IX_ArtLease_LeaseUserId",
                table: "ArtLease");

            migrationBuilder.DropIndex(
                name: "IX_ArtLease_OwnerUserId",
                table: "ArtLease");

            migrationBuilder.DropColumn(
                name: "LeaseUserId",
                table: "ArtLease");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "ArtLease");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LeaseUserId",
                table: "ArtLease",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerUserId",
                table: "ArtLease",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArtLease_LeaseUserId",
                table: "ArtLease",
                column: "LeaseUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtLease_OwnerUserId",
                table: "ArtLease",
                column: "OwnerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtLease_AspNetUsers_LeaseUserId",
                table: "ArtLease",
                column: "LeaseUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtLease_AspNetUsers_OwnerUserId",
                table: "ArtLease",
                column: "OwnerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
