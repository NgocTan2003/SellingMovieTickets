using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SellingMovieTickets.Migrations
{
    public partial class updateTBCinemaShowTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MovieSreenTime",
                table: "CinemaShowTimes",
                newName: "StartShowTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndShowTime",
                table: "CinemaShowTimes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndShowTime",
                table: "CinemaShowTimes");

            migrationBuilder.RenameColumn(
                name: "StartShowTime",
                table: "CinemaShowTimes",
                newName: "MovieSreenTime");
        }
    }
}
