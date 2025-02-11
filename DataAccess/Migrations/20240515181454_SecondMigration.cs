using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_AspNetUsers_UserId",
                table: "Permissions");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Permissions",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_Permissions_UserId",
                table: "Permissions",
                newName: "IX_Permissions_RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_AspNetRoles_RoleId",
                table: "Permissions",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_AspNetRoles_RoleId",
                table: "Permissions");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "Permissions",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Permissions_RoleId",
                table: "Permissions",
                newName: "IX_Permissions_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_AspNetUsers_UserId",
                table: "Permissions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
