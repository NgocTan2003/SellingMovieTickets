using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SellingMovieTickets.Migrations
{
    public partial class updateTBOrtherService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "OtherServices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "OtherServices");
        }
    }
}
