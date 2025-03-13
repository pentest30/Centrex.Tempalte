using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saylo.Centrex.Identity.Core.Migrations
{
    /// <inheritdoc />
    public partial class mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "Admin",
                table: "IdentityUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "Admin",
                table: "IdentityUser",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
