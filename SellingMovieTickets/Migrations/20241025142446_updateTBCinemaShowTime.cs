using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SellingMovieTickets.Migrations
{
    public partial class updateTBCinemaShowTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CinemaShowTimes_CinemaShowTimes_CinemaShowTimeModelId",
                table: "CinemaShowTimes");

            migrationBuilder.DropIndex(
                name: "IX_CinemaShowTimes_CinemaShowTimeModelId",
                table: "CinemaShowTimes");

            migrationBuilder.DropColumn(
                name: "CinemaShowTimeModelId",
                table: "CinemaShowTimes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CinemaShowTimeModelId",
                table: "CinemaShowTimes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CinemaShowTimes_CinemaShowTimeModelId",
                table: "CinemaShowTimes",
                column: "CinemaShowTimeModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_CinemaShowTimes_CinemaShowTimes_CinemaShowTimeModelId",
                table: "CinemaShowTimes",
                column: "CinemaShowTimeModelId",
                principalTable: "CinemaShowTimes",
                principalColumn: "Id");
        }
    }
}
