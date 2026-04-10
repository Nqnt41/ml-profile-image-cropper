using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfileSearch.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCaps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "y",
                table: "Images",
                newName: "Y");

            migrationBuilder.RenameColumn(
                name: "x",
                table: "Images",
                newName: "X");

            migrationBuilder.RenameColumn(
                name: "width",
                table: "Images",
                newName: "Width");

            migrationBuilder.RenameColumn(
                name: "height",
                table: "Images",
                newName: "Height");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Y",
                table: "Images",
                newName: "y");

            migrationBuilder.RenameColumn(
                name: "X",
                table: "Images",
                newName: "x");

            migrationBuilder.RenameColumn(
                name: "Width",
                table: "Images",
                newName: "width");

            migrationBuilder.RenameColumn(
                name: "Height",
                table: "Images",
                newName: "height");
        }
    }
}
