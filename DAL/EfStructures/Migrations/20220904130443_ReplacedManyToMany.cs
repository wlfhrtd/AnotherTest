using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.EfStructures.Migrations
{
    public partial class ReplacedManyToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentMap");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentMainName",
                table: "Departments",
                type: "character varying(50)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DepartmentMainName",
                table: "Departments",
                column: "DepartmentMainName");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Departments_DepartmentMainName",
                table: "Departments",
                column: "DepartmentMainName",
                principalTable: "Departments",
                principalColumn: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Departments_DepartmentMainName",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_DepartmentMainName",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DepartmentMainName",
                table: "Departments");

            migrationBuilder.CreateTable(
                name: "DepartmentMap",
                columns: table => new
                {
                    DepartmentName = table.Column<string>(type: "character varying(50)", nullable: false),
                    SubdepartmentName = table.Column<string>(type: "character varying(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentMap", x => new { x.DepartmentName, x.SubdepartmentName });
                    table.ForeignKey(
                        name: "FK_DepartmentMap_Departments_DepartmentName",
                        column: x => x.DepartmentName,
                        principalTable: "Departments",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartmentMap_Departments_SubdepartmentName",
                        column: x => x.SubdepartmentName,
                        principalTable: "Departments",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentMap_SubdepartmentName",
                table: "DepartmentMap",
                column: "SubdepartmentName");
        }
    }
}
