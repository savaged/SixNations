using Microsoft.EntityFrameworkCore.Migrations;

namespace SixNations.Server.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "RequirementEstimation",
                columns: new[] { "RequirementEstimationID", "RequirementEstimationName" },
                values: new object[,]
                {
                    { 1, "XS" },
                    { 2, "Small" },
                    { 3, "Medium" },
                    { 5, "Large" },
                    { 8, "XL" },
                    { 13, "XXL" }
                });

            migrationBuilder.InsertData(
                table: "RequirementPriority",
                columns: new[] { "RequirementPriorityID", "RequirementPriorityName" },
                values: new object[,]
                {
                    { 1, "Must" },
                    { 2, "Should" },
                    { 3, "Could" },
                    { 4, "Wont" }
                });

            migrationBuilder.InsertData(
                table: "RequirementStatus",
                columns: new[] { "RequirementStatusID", "RequirementStatusName" },
                values: new object[,]
                {
                    { 1, "Prioritised" },
                    { 2, "WIP" },
                    { 3, "Test" },
                    { 4, "Done" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RequirementEstimation",
                keyColumn: "RequirementEstimationID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RequirementEstimation",
                keyColumn: "RequirementEstimationID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RequirementEstimation",
                keyColumn: "RequirementEstimationID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "RequirementEstimation",
                keyColumn: "RequirementEstimationID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "RequirementEstimation",
                keyColumn: "RequirementEstimationID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "RequirementEstimation",
                keyColumn: "RequirementEstimationID",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "RequirementPriority",
                keyColumn: "RequirementPriorityID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RequirementPriority",
                keyColumn: "RequirementPriorityID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RequirementPriority",
                keyColumn: "RequirementPriorityID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "RequirementPriority",
                keyColumn: "RequirementPriorityID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "RequirementStatus",
                keyColumn: "RequirementStatusID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RequirementStatus",
                keyColumn: "RequirementStatusID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RequirementStatus",
                keyColumn: "RequirementStatusID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "RequirementStatus",
                keyColumn: "RequirementStatusID",
                keyValue: 4);
        }
    }
}
