using Microsoft.EntityFrameworkCore.Migrations;

namespace Exchange_Art.Migrations
{
    public partial class UpdatedArtLeaseModelagain2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeasePeriodInDays",
                table: "ArtLease");

            migrationBuilder.AddColumn<int>(
                name: "LeasePeriodInMonths",
                table: "ArtLease",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeasePeriodInMonths",
                table: "ArtLease");

            migrationBuilder.AddColumn<int>(
                name: "LeasePeriodInDays",
                table: "ArtLease",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
