using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Produccion.Migrations
{
    public partial class FabricMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fabrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fabrics", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Colors_Nombre",
                table: "Colors",
                column: "Nombre",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fabrics");

            migrationBuilder.DropIndex(
                name: "IX_Colors_Nombre",
                table: "Colors");
        }
    }
}
