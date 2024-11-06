using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SellingMovieTickets.Migrations
{
    public partial class updateDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OtherServicesOrders_AspNetUsers_UserId1",
                table: "OtherServicesOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_CustomerManagements_CustomerManagementModelId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_CustomerManagementModelId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_OtherServicesOrders_UserId1",
                table: "OtherServicesOrders");

            migrationBuilder.DropColumn(
                name: "CustomerManagementModelId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "OtherServicesOrders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "OtherServicesOrders");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "OtherServicesOrders");

            migrationBuilder.DropColumn(
                name: "ConcessionAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RoomNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CurrentPointsBalance",
                table: "CustomerPoints");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CustomerPoints");

            migrationBuilder.DropColumn(
                name: "LastTransactionDate",
                table: "CustomerPoints");

            migrationBuilder.RenameColumn(
                name: "SeatNumber",
                table: "Tickets",
                newName: "SeatNames");

            migrationBuilder.RenameColumn(
                name: "LoyaltyPoints",
                table: "CustomerManagements",
                newName: "CurrentPointsBalance");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "OtherServicesOrders",
                type: "decimal(10,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Orders",
                type: "decimal(10,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTickets",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OtherServicesOrderId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerPointsHistoryModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    PointsChanged = table.Column<int>(type: "int", nullable: false),
                    BalanceAfterTransaction = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PointChangeStatus = table.Column<int>(type: "int", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPointsHistoryModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerPointsHistoryModel_CustomerManagements_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CustomerManagements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OtherServicesOrderId",
                table: "Orders",
                column: "OtherServicesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPointsHistoryModel_CustomerId",
                table: "CustomerPointsHistoryModel",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OtherServicesOrders_OtherServicesOrderId",
                table: "Orders",
                column: "OtherServicesOrderId",
                principalTable: "OtherServicesOrders",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OtherServicesOrders_OtherServicesOrderId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "CustomerPointsHistoryModel");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OtherServicesOrderId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "OtherServicesOrders");

            migrationBuilder.DropColumn(
                name: "NumberOfTickets",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OtherServicesOrderId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "SeatNames",
                table: "Tickets",
                newName: "SeatNumber");

            migrationBuilder.RenameColumn(
                name: "CurrentPointsBalance",
                table: "CustomerManagements",
                newName: "LoyaltyPoints");

            migrationBuilder.AddColumn<int>(
                name: "CustomerManagementModelId",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                table: "OtherServicesOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "OtherServicesOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "OtherServicesOrders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,3)");

            migrationBuilder.AddColumn<decimal>(
                name: "ConcessionAmount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentAmount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RoomNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CurrentPointsBalance",
                table: "CustomerPoints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CustomerPoints",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTransactionDate",
                table: "CustomerPoints",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CustomerManagementModelId",
                table: "Tickets",
                column: "CustomerManagementModelId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherServicesOrders_UserId1",
                table: "OtherServicesOrders",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OtherServicesOrders_AspNetUsers_UserId1",
                table: "OtherServicesOrders",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_CustomerManagements_CustomerManagementModelId",
                table: "Tickets",
                column: "CustomerManagementModelId",
                principalTable: "CustomerManagements",
                principalColumn: "Id");
        }
    }
}
