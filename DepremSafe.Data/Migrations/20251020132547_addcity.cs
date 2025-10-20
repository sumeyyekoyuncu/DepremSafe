using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepremSafe.Data.Migrations
{
    /// <inheritdoc />
    public partial class addcity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "tbl_UserLocation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "tbl_UserLocation");
        }
    }
}
