using Microsoft.EntityFrameworkCore.Migrations;

namespace FinalYearProject.Migrations
{
    public partial class AddedUserTableAndAddedPluralToTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ControlEvaluationControl_Control_ControlId",
                table: "ControlEvaluationControl");

            migrationBuilder.DropForeignKey(
                name: "FK_ControlEvaluationControl_ControlEvaluation_ControlEvaluationsId",
                table: "ControlEvaluationControl");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ControlEvaluationControl",
                table: "ControlEvaluationControl");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ControlEvaluation",
                table: "ControlEvaluation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Control",
                table: "Control");

            migrationBuilder.RenameTable(
                name: "ControlEvaluationControl",
                newName: "ControlEvaluationControls");

            migrationBuilder.RenameTable(
                name: "ControlEvaluation",
                newName: "ControlEvaluations");

            migrationBuilder.RenameTable(
                name: "Control",
                newName: "Controls");

            migrationBuilder.RenameIndex(
                name: "IX_ControlEvaluationControl_ControlId",
                table: "ControlEvaluationControls",
                newName: "IX_ControlEvaluationControls_ControlId");

            migrationBuilder.RenameIndex(
                name: "IX_ControlEvaluationControl_ControlEvaluationsId",
                table: "ControlEvaluationControls",
                newName: "IX_ControlEvaluationControls_ControlEvaluationsId");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ControlEvaluations",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ControlEvaluationControls",
                table: "ControlEvaluationControls",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ControlEvaluations",
                table: "ControlEvaluations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Controls",
                table: "Controls",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserRole = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ControlEvaluations_UserId",
                table: "ControlEvaluations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ControlEvaluationControls_ControlEvaluations_ControlEvaluationsId",
                table: "ControlEvaluationControls",
                column: "ControlEvaluationsId",
                principalTable: "ControlEvaluations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ControlEvaluationControls_Controls_ControlId",
                table: "ControlEvaluationControls",
                column: "ControlId",
                principalTable: "Controls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ControlEvaluations_Users_UserId",
                table: "ControlEvaluations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ControlEvaluationControls_ControlEvaluations_ControlEvaluationsId",
                table: "ControlEvaluationControls");

            migrationBuilder.DropForeignKey(
                name: "FK_ControlEvaluationControls_Controls_ControlId",
                table: "ControlEvaluationControls");

            migrationBuilder.DropForeignKey(
                name: "FK_ControlEvaluations_Users_UserId",
                table: "ControlEvaluations");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Controls",
                table: "Controls");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ControlEvaluations",
                table: "ControlEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_ControlEvaluations_UserId",
                table: "ControlEvaluations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ControlEvaluationControls",
                table: "ControlEvaluationControls");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ControlEvaluations");

            migrationBuilder.RenameTable(
                name: "Controls",
                newName: "Control");

            migrationBuilder.RenameTable(
                name: "ControlEvaluations",
                newName: "ControlEvaluation");

            migrationBuilder.RenameTable(
                name: "ControlEvaluationControls",
                newName: "ControlEvaluationControl");

            migrationBuilder.RenameIndex(
                name: "IX_ControlEvaluationControls_ControlId",
                table: "ControlEvaluationControl",
                newName: "IX_ControlEvaluationControl_ControlId");

            migrationBuilder.RenameIndex(
                name: "IX_ControlEvaluationControls_ControlEvaluationsId",
                table: "ControlEvaluationControl",
                newName: "IX_ControlEvaluationControl_ControlEvaluationsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Control",
                table: "Control",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ControlEvaluation",
                table: "ControlEvaluation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ControlEvaluationControl",
                table: "ControlEvaluationControl",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ControlEvaluationControl_Control_ControlId",
                table: "ControlEvaluationControl",
                column: "ControlId",
                principalTable: "Control",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ControlEvaluationControl_ControlEvaluation_ControlEvaluationsId",
                table: "ControlEvaluationControl",
                column: "ControlEvaluationsId",
                principalTable: "ControlEvaluation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
