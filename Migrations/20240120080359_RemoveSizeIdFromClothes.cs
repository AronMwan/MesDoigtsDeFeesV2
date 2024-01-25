using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MesDoigtsDeFees.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSizeIdFromClothes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_Clothes_Sizes_SizeId",
                table: "Clothes");

            // Drop the index
            migrationBuilder.DropIndex(
                name: "IX_Clothes_SizeId",
                table: "Clothes");

            // Drop the SizeId column
            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "Clothes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add the SizeId column back
            migrationBuilder.AddColumn<int>(
                name: "SizeId",
                table: "Clothes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Recreate the index
            migrationBuilder.CreateIndex(
                name: "IX_Clothes_SizeId",
                table: "Clothes",
                column: "SizeId");

            // Recreate the foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_Clothes_Sizes_SizeId",
                table: "Clothes",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }

}
