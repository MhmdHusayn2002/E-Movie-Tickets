using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eTickets.Migrations
{
    /// <inheritdoc />
    public partial class Emovie1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailableTickets",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableTickets",
                table: "Movies");
        }
    }
}
