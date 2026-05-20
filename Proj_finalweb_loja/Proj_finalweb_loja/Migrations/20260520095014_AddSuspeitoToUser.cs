using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proj_finalweb_loja.Migrations
{
    /// <inheritdoc />
    public partial class AddSuspeitoToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Suspeito",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Suspeito",
                table: "AspNetUsers");
        }
    }
}
