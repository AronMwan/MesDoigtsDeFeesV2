using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MesDoigtsDeFees.Migrations
{
    /// <inheritdoc />
    public partial class GroupMaker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupMakerId",
                table: "Groups",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_GroupMakerId",
                table: "Groups",
                column: "GroupMakerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_AspNetUsers_GroupMakerId",
                table: "Groups",
                column: "GroupMakerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_AspNetUsers_GroupMakerId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_GroupMakerId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "GroupMakerId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Groups");
        }
    }
}
