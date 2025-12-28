using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepremSafe.Data.Migrations
{
    /// <inheritdoc />
    public partial class columnemail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "tbl_User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GoogleId",
                table: "tbl_User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoginProvider",
                table: "tbl_User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "tbl_User");

            migrationBuilder.DropColumn(
                name: "GoogleId",
                table: "tbl_User");

            migrationBuilder.DropColumn(
                name: "LoginProvider",
                table: "tbl_User");
        }
    }
}
