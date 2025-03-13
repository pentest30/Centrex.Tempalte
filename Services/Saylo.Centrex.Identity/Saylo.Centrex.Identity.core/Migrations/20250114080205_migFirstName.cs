using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saylo.Centrex.Identity.Core.Migrations
{
    /// <inheritdoc />
    public partial class migFirstName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "Admin",
                table: "IdentityUser",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "Admin",
                table: "IdentityUser");
        }
    }
}
