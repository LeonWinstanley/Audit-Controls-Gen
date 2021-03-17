using Microsoft.EntityFrameworkCore.Migrations;

namespace FinalYearProject.Migrations
{
    public partial class AddedNewTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Controls");

            migrationBuilder.CreateTable(
                name: "Control",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ControlSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ControlExpected = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ControlTest = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Control", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ControlEvaluation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlEvaluation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ControlEvaluationControl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ControlId = table.Column<int>(type: "int", nullable: true),
                    ControlEvaluationsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlEvaluationControl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ControlEvaluationControl_Control_ControlId",
                        column: x => x.ControlId,
                        principalTable: "Control",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ControlEvaluationControl_ControlEvaluation_ControlEvaluationsId",
                        column: x => x.ControlEvaluationsId,
                        principalTable: "ControlEvaluation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ControlEvaluationControl_ControlEvaluationsId",
                table: "ControlEvaluationControl",
                column: "ControlEvaluationsId");

            migrationBuilder.CreateIndex(
                name: "IX_ControlEvaluationControl_ControlId",
                table: "ControlEvaluationControl",
                column: "ControlId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ControlEvaluationControl");

            migrationBuilder.DropTable(
                name: "Control");

            migrationBuilder.DropTable(
                name: "ControlEvaluation");

            migrationBuilder.CreateTable(
                name: "Controls",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ControlExpectedValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ControlSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ControlTestValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Controls", x => x.id);
                });
        }
    }
}
