using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ErrorReporter.Migrations
{
    /// <inheritdoc />
    public partial class LinkErrorReportsToApiClients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApiClientId",
                table: "ErrorReports",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ErrorReports_ApiClientId",
                table: "ErrorReports",
                column: "ApiClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_ErrorReports_ApiClients_ApiClientId",
                table: "ErrorReports",
                column: "ApiClientId",
                principalTable: "ApiClients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ErrorReports_ApiClients_ApiClientId",
                table: "ErrorReports");

            migrationBuilder.DropIndex(
                name: "IX_ErrorReports_ApiClientId",
                table: "ErrorReports");

            migrationBuilder.DropColumn(
                name: "ApiClientId",
                table: "ErrorReports");
        }
    }
}
