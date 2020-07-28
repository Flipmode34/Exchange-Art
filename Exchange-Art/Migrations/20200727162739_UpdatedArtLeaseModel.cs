using Microsoft.EntityFrameworkCore.Migrations;

namespace Exchange_Art.Migrations
{
    public partial class UpdatedArtLeaseModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerName",
                table: "ArtLease",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerName",
                table: "ArtLease");
        }
    }
}
