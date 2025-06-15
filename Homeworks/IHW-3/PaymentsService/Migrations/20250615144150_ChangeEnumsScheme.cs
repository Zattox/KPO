using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentsService.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEnumsScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PaymentTransactions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Accounts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_OrderId",
                table: "PaymentTransactions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_UserId_CreatedAt",
                table: "PaymentTransactions",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Type_ProcessedOn",
                table: "OutboxMessages",
                columns: new[] { "Type", "ProcessedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_InboxMessages_Type_ProcessedOn",
                table: "InboxMessages",
                columns: new[] { "Type", "ProcessedOn" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_OrderId",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_UserId_CreatedAt",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_Type_ProcessedOn",
                table: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_InboxMessages_Type_ProcessedOn",
                table: "InboxMessages");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PaymentTransactions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Accounts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}
