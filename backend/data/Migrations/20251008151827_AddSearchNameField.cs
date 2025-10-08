using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchNameField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SearchFullName",
                table: "PlayersStaging",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SearchFullName",
                table: "Players",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SearchFullName",
                table: "PlayersStaging");

            migrationBuilder.DropColumn(
                name: "SearchFullName",
                table: "Players");
        }
    }
}
