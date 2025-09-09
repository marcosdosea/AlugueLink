using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlugueLinkWEB.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTipoUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoUsuario",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoUsuario",
                table: "AspNetUsers",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
