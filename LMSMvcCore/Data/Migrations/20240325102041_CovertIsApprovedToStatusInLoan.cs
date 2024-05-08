using Microsoft.EntityFrameworkCore.Migrations;

namespace LMSMvcCore.Data.Migrations
{
    public partial class CovertIsApprovedToStatusInLoan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Loans");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Loans",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Loans");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Loans",
                type: "bit",
                nullable: true);
        }
    }
}
