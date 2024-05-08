using Microsoft.EntityFrameworkCore.Migrations;

namespace LMSMvcCore.Data.Migrations
{
    public partial class RefreshTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IPPSNumber",
                table: "Loans");

            migrationBuilder.RenameColumn(
                name: "Organization",
                table: "Loans",
                newName: "PVN");

            migrationBuilder.AddColumn<long>(
                name: "AccountNumber",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "PVN",
                table: "Loans",
                newName: "Organization");

            migrationBuilder.AddColumn<string>(
                name: "IPPSNumber",
                table: "Loans",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
