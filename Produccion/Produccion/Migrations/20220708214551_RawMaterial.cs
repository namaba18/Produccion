using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Produccion.Migrations
{
    public partial class RawMaterial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RawMaterialId",
                table: "Fabrics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RawMaterialId",
                table: "Colors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RawMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawMaterials", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fabrics_RawMaterialId",
                table: "Fabrics",
                column: "RawMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Colors_RawMaterialId",
                table: "Colors",
                column: "RawMaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_Colors_RawMaterials_RawMaterialId",
                table: "Colors",
                column: "RawMaterialId",
                principalTable: "RawMaterials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Fabrics_RawMaterials_RawMaterialId",
                table: "Fabrics",
                column: "RawMaterialId",
                principalTable: "RawMaterials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colors_RawMaterials_RawMaterialId",
                table: "Colors");

            migrationBuilder.DropForeignKey(
                name: "FK_Fabrics_RawMaterials_RawMaterialId",
                table: "Fabrics");

            migrationBuilder.DropTable(
                name: "RawMaterials");

            migrationBuilder.DropIndex(
                name: "IX_Fabrics_RawMaterialId",
                table: "Fabrics");

            migrationBuilder.DropIndex(
                name: "IX_Colors_RawMaterialId",
                table: "Colors");

            migrationBuilder.DropColumn(
                name: "RawMaterialId",
                table: "Fabrics");

            migrationBuilder.DropColumn(
                name: "RawMaterialId",
                table: "Colors");
        }
    }
}
