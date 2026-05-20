using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proj_finalweb_loja.Migrations
{
    /// <inheritdoc />
    public partial class AddVendedoresFavoritos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VendedoresFavoritos",
                columns: table => new
                {
                    SeguidorFK = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    VendedorFK = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    DataAdicionado = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendedoresFavoritos", x => new { x.SeguidorFK, x.VendedorFK });
                    table.ForeignKey(
                        name: "FK_VendedoresFavoritos_AspNetUsers_SeguidorFK",
                        column: x => x.SeguidorFK,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendedoresFavoritos_AspNetUsers_VendedorFK",
                        column: x => x.VendedorFK,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VendedoresFavoritos_VendedorFK",
                table: "VendedoresFavoritos",
                column: "VendedorFK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendedoresFavoritos");
        }
    }
}
