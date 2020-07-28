using Microsoft.EntityFrameworkCore.Migrations;

namespace Exchange_Art.Migrations
{
    public partial class UpdatedArtLeaseModelAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeaseDaysleft",
                table: "ArtLease");

            migrationBuilder.DropColumn(
                name: "LeasePeriod",
                table: "ArtLease");

            migrationBuilder.AddColumn<string>(
                name: "DateLeaseEnds",
                table: "ArtLease",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateLeaseStarted",
                table: "ArtLease",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LeasePeriodInDays",
                table: "ArtLease",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateLeaseEnds",
                table: "ArtLease");

            migrationBuilder.DropColumn(
                name: "DateLeaseStarted",
                table: "ArtLease");

            migrationBuilder.DropColumn(
                name: "LeasePeriodInDays",
                table: "ArtLease");

            migrationBuilder.AddColumn<int>(
                name: "LeaseDaysleft",
                table: "ArtLease",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LeasePeriod",
                table: "ArtLease",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
