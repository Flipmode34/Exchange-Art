using Microsoft.EntityFrameworkCore.Migrations;

namespace Exchange_Art.Migrations
{
    public partial class ArtLeaseModelAndArtModelUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "LeasePrice",
                table: "Art",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "ArtLease",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArtPiece = table.Column<string>(nullable: true),
                    ArtPieceId = table.Column<int>(nullable: true),
                    ArtLeaser = table.Column<string>(nullable: true),
                    LeaseUserId = table.Column<string>(nullable: true),
                    LeasePeriod = table.Column<int>(nullable: false),
                    LeaseDaysleft = table.Column<int>(nullable: false),
                    LeaseAmount = table.Column<double>(nullable: false),
                    ArtOwner = table.Column<string>(nullable: true),
                    OwnerUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtLease", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtLease_Art_ArtPieceId",
                        column: x => x.ArtPieceId,
                        principalTable: "Art",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArtLease_AspNetUsers_LeaseUserId",
                        column: x => x.LeaseUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArtLease_AspNetUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtLease_ArtPieceId",
                table: "ArtLease",
                column: "ArtPieceId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtLease_LeaseUserId",
                table: "ArtLease",
                column: "LeaseUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtLease_OwnerUserId",
                table: "ArtLease",
                column: "OwnerUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtLease");

            migrationBuilder.DropColumn(
                name: "LeasePrice",
                table: "Art");
        }
    }
}
