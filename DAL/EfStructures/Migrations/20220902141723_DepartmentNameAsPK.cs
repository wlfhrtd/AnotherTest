using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.EfStructures.Migrations
{
    public partial class DepartmentNameAsPK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentMap_Departments_DepartmentId",
                table: "DepartmentMap");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentMap_Departments_SubdepartmentId",
                table: "DepartmentMap");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Departments",
                table: "Departments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DepartmentMap",
                table: "DepartmentMap");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentMap_SubdepartmentId",
                table: "DepartmentMap");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "DepartmentMap");

            migrationBuilder.DropColumn(
                name: "SubdepartmentId",
                table: "DepartmentMap");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "DepartmentMap",
                type: "character varying(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubdepartmentName",
                table: "DepartmentMap",
                type: "character varying(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Departments",
                table: "Departments",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DepartmentMap",
                table: "DepartmentMap",
                columns: new[] { "DepartmentName", "SubdepartmentName" });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentMap_SubdepartmentName",
                table: "DepartmentMap",
                column: "SubdepartmentName");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentMap_Departments_DepartmentName",
                table: "DepartmentMap",
                column: "DepartmentName",
                principalTable: "Departments",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentMap_Departments_SubdepartmentName",
                table: "DepartmentMap",
                column: "SubdepartmentName",
                principalTable: "Departments",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentMap_Departments_DepartmentName",
                table: "DepartmentMap");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentMap_Departments_SubdepartmentName",
                table: "DepartmentMap");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Departments",
                table: "Departments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DepartmentMap",
                table: "DepartmentMap");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentMap_SubdepartmentName",
                table: "DepartmentMap");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "DepartmentMap");

            migrationBuilder.DropColumn(
                name: "SubdepartmentName",
                table: "DepartmentMap");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Departments",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "DepartmentMap",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubdepartmentId",
                table: "DepartmentMap",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Departments",
                table: "Departments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DepartmentMap",
                table: "DepartmentMap",
                columns: new[] { "DepartmentId", "SubdepartmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentMap_SubdepartmentId",
                table: "DepartmentMap",
                column: "SubdepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentMap_Departments_DepartmentId",
                table: "DepartmentMap",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentMap_Departments_SubdepartmentId",
                table: "DepartmentMap",
                column: "SubdepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
