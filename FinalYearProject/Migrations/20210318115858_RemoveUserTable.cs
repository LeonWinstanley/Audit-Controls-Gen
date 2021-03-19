using Microsoft.EntityFrameworkCore.Migrations;

namespace FinalYearProject.Migrations
{
    public partial class RemoveUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ControlEvaluations_Users_UserId",
                table: "ControlEvaluations");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_ControlEvaluations_UserId",
                table: "ControlEvaluations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ControlEvaluations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ControlEvaluations",
                type: "int",
                nullable: true);

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
                name: "FK_ControlEvaluations_Users_UserId",
                table: "ControlEvaluations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
