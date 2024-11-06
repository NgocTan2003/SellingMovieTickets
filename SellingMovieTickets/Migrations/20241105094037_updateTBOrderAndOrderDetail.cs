using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SellingMovieTickets.Migrations
{
    public partial class updateTBOrderAndOrderDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Tickets_TicketId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_AppUserId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_CinemaShowTimes_CinemaShowTimeId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Promotions_PromotionId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_AppUserId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_CinemaShowTimeId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_TicketId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CinemaShowTimeId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "TicketType",
                table: "Tickets",
                newName: "TicketCode");

            migrationBuilder.RenameColumn(
                name: "PurchaseDate",
                table: "Tickets",
                newName: "StartShowTime");

            migrationBuilder.RenameColumn(
                name: "PromotionId",
                table: "Tickets",
                newName: "PromotionModelId");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Tickets",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "PaymentStatus",
                table: "Tickets",
                newName: "NameMovie");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_PromotionId",
                table: "Tickets",
                newName: "IX_Tickets_PromotionModelId");

            migrationBuilder.AddColumn<int>(
                name: "CinemaShowTimeModelId",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ConcessionAmount",
                table: "Tickets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "Tickets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentAmount",
                table: "Tickets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentTime",
                table: "Tickets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CinemaShowTimeId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TicketId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SeatNumber",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CinemaShowTimeModelId",
                table: "Tickets",
                column: "CinemaShowTimeModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CinemaShowTimeId",
                table: "Orders",
                column: "CinemaShowTimeId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TicketId",
                table: "Orders",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_CinemaShowTimes_CinemaShowTimeId",
                table: "Orders",
                column: "CinemaShowTimeId",
                principalTable: "CinemaShowTimes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Tickets_TicketId",
                table: "Orders",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_CinemaShowTimes_CinemaShowTimeModelId",
                table: "Tickets",
                column: "CinemaShowTimeModelId",
                principalTable: "CinemaShowTimes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Promotions_PromotionModelId",
                table: "Tickets",
                column: "PromotionModelId",
                principalTable: "Promotions",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_CinemaShowTimes_CinemaShowTimeId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Tickets_TicketId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_CinemaShowTimes_CinemaShowTimeModelId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Promotions_PromotionModelId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_CinemaShowTimeModelId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CinemaShowTimeId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TicketId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CinemaShowTimeModelId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ConcessionAmount",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "PaymentAmount",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "PaymentTime",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CinemaShowTimeId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RoomNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SeatNumber",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Tickets",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "TicketCode",
                table: "Tickets",
                newName: "TicketType");

            migrationBuilder.RenameColumn(
                name: "StartShowTime",
                table: "Tickets",
                newName: "PurchaseDate");

            migrationBuilder.RenameColumn(
                name: "PromotionModelId",
                table: "Tickets",
                newName: "PromotionId");

            migrationBuilder.RenameColumn(
                name: "NameMovie",
                table: "Tickets",
                newName: "PaymentStatus");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_PromotionModelId",
                table: "Tickets",
                newName: "IX_Tickets_PromotionId");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Tickets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CinemaShowTimeId",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TicketId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AppUserId",
                table: "Tickets",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CinemaShowTimeId",
                table: "Tickets",
                column: "CinemaShowTimeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_TicketId",
                table: "OrderDetails",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Tickets_TicketId",
                table: "OrderDetails",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_AppUserId",
                table: "Tickets",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_CinemaShowTimes_CinemaShowTimeId",
                table: "Tickets",
                column: "CinemaShowTimeId",
                principalTable: "CinemaShowTimes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Promotions_PromotionId",
                table: "Tickets",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "Id");
        }
    }
}
