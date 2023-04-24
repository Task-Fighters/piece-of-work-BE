using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace salty_server.Migrations
{
    /// <inheritdoc />
    public partial class AddRepoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupUser",
                table: "GroupUser");

            migrationBuilder.DropIndex(
                name: "IX_GroupUser_UsersId",
                table: "GroupUser");

            migrationBuilder.AlterColumn<int>(
                name: "UsersId",
                table: "GroupUser",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<int>(
                name: "GroupsId",
                table: "GroupUser",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupUser",
                table: "GroupUser",
                columns: new[] { "UsersId", "GroupsId" });

            migrationBuilder.CreateTable(
                name: "Repos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignmentId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Repos_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Repos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupUser_GroupsId",
                table: "GroupUser",
                column: "GroupsId");

            migrationBuilder.CreateIndex(
                name: "IX_Repos_AssignmentId",
                table: "Repos",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Repos_UserId",
                table: "Repos",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Repos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupUser",
                table: "GroupUser");

            migrationBuilder.DropIndex(
                name: "IX_GroupUser_GroupsId",
                table: "GroupUser");

            migrationBuilder.AlterColumn<int>(
                name: "GroupsId",
                table: "GroupUser",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "UsersId",
                table: "GroupUser",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupUser",
                table: "GroupUser",
                columns: new[] { "GroupsId", "UsersId" });

            migrationBuilder.CreateIndex(
                name: "IX_GroupUser_UsersId",
                table: "GroupUser",
                column: "UsersId");
        }
    }
}
