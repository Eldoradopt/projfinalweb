using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proj_finalweb_loja.Migrations
{
    /// <inheritdoc />
    public partial class AddTagsAndColecoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacoes_Anuncios_AnuncioFK",
                table: "Avaliacoes");

            migrationBuilder.AlterColumn<int>(
                name: "AnuncioFK",
                table: "Avaliacoes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EColecaoEspecial = table.Column<bool>(type: "bit", nullable: false),
                    Icone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CorHex = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnuncioTags",
                columns: table => new
                {
                    AnuncioFK = table.Column<int>(type: "int", nullable: false),
                    TagFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnuncioTags", x => new { x.AnuncioFK, x.TagFK });
                    table.ForeignKey(
                        name: "FK_AnuncioTags_Anuncios_AnuncioFK",
                        column: x => x.AnuncioFK,
                        principalTable: "Anuncios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnuncioTags_Tags_TagFK",
                        column: x => x.TagFK,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnuncioTags_TagFK",
                table: "AnuncioTags",
                column: "TagFK");

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacoes_Anuncios_AnuncioFK",
                table: "Avaliacoes",
                column: "AnuncioFK",
                principalTable: "Anuncios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacoes_Anuncios_AnuncioFK",
                table: "Avaliacoes");

            migrationBuilder.DropTable(
                name: "AnuncioTags");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.AlterColumn<int>(
                name: "AnuncioFK",
                table: "Avaliacoes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacoes_Anuncios_AnuncioFK",
                table: "Avaliacoes",
                column: "AnuncioFK",
                principalTable: "Anuncios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
