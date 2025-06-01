using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechGalaxyProject.Migrations
{
    /// <inheritdoc />
    public partial class twomigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DifficultyLevel",
                table: "fields");

            migrationBuilder.AddColumn<string>(
                name: "DifficultyLevel",
                table: "roadmaps",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "fieldResources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fieldResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_fieldResources_fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_fieldResources_FieldId",
                table: "fieldResources",
                column: "FieldId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fieldResources");

            migrationBuilder.DropColumn(
                name: "DifficultyLevel",
                table: "roadmaps");

            migrationBuilder.AddColumn<string>(
                name: "DifficultyLevel",
                table: "fields",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
