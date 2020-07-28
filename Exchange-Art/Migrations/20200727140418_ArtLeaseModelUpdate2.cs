using Microsoft.EntityFrameworkCore.Migrations;

namespace Exchange_Art.Migrations
{
    public partial class ArtLeaseModelUpdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LeaserName",
                table: "ArtLease",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeaserName",
                table: "ArtLease");
        }
    }
}
