using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace salty_server.Migrations
{
    /// <inheritdoc />
    public partial class AddBootcampToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bootcamp",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bootcamp",
                table: "Users");
        }
    }
}
