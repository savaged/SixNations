using Microsoft.EntityFrameworkCore.Migrations;

namespace SixNations.Server.Migrations
{
    public partial class AddedRequirementStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "RequirementStatus",
                columns: new[] { "RequirementStatusID", "RequirementStatusName" },
                values: new object[] { 5, "Reviewed" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RequirementStatus",
                keyColumn: "RequirementStatusID",
                keyValue: 5);
        }
    }
}
