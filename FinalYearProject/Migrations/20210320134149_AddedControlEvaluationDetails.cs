using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FinalYearProject.Migrations
{
    public partial class AddedControlEvaluationDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuditName",
                table: "ControlEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateCreated",
                table: "ControlEvaluations",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "LeadAuditor",
                table: "ControlEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewerEmail",
                table: "ControlEvaluations",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuditName",
                table: "ControlEvaluations");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "ControlEvaluations");

            migrationBuilder.DropColumn(
                name: "LeadAuditor",
                table: "ControlEvaluations");

            migrationBuilder.DropColumn(
                name: "ReviewerEmail",
                table: "ControlEvaluations");
        }
    }
}
