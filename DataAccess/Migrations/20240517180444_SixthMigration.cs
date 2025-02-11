using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SixthMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_AspNetRoles_RoleId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "CreateListing",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Download",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Upload",
                table: "Permissions");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "Permissions",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Permissions_RoleId",
                table: "Permissions",
                newName: "IX_Permissions_UserId");

            migrationBuilder.AddColumn<int>(
                name: "CVId",
                table: "Permissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_CVId",
                table: "Permissions",
                column: "CVId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_AspNetUsers_UserId",
                table: "Permissions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_CVs_CVId",
                table: "Permissions",
                column: "CVId",
                principalTable: "CVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_AspNetUsers_UserId",
                table: "Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_CVs_CVId",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_CVId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "CVId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Permissions");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Permissions",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_Permissions_UserId",
                table: "Permissions",
                newName: "IX_Permissions_RoleId");

            migrationBuilder.AddColumn<bool>(
                name: "CreateListing",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Download",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Upload",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_AspNetRoles_RoleId",
                table: "Permissions",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
