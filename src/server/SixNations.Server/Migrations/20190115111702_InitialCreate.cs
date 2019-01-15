using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SixNations.Server.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Requirement",
                columns: table => new
                {
                    RequirementID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Story = table.Column<string>(nullable: false),
                    Release = table.Column<string>(nullable: true),
                    Estimation = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requirement", x => x.RequirementID);
                });

            migrationBuilder.CreateTable(
                name: "RequirementEstimation",
                columns: table => new
                {
                    RequirementEstimationID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RequirementEstimationName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequirementEstimation", x => x.RequirementEstimationID);
                });

            migrationBuilder.CreateTable(
                name: "RequirementPriority",
                columns: table => new
                {
                    RequirementPriorityID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RequirementPriorityName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequirementPriority", x => x.RequirementPriorityID);
                });

            migrationBuilder.CreateTable(
                name: "RequirementStatus",
                columns: table => new
                {
                    RequirementStatusID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RequirementStatusName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequirementStatus", x => x.RequirementStatusID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(nullable: true),
                    Firstname = table.Column<string>(nullable: false),
                    Lastname = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    access_token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_User_Username",
                table: "User",
                column: "Username",
                unique: true,
                filter: "[Username] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Requirement");

            migrationBuilder.DropTable(
                name: "RequirementEstimation");

            migrationBuilder.DropTable(
                name: "RequirementPriority");

            migrationBuilder.DropTable(
                name: "RequirementStatus");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
