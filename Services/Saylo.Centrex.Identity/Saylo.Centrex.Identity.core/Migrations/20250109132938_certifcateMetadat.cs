using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saylo.Centrex.Identity.Core.Migrations
{
    /// <inheritdoc />
    public partial class certifcateMetadat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CertificateMetadata",
                schema: "Admin",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Filename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Thumbprint = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Kid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Kty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Use = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    N = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    E = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateById = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateMetadata", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CertificateMetadata_IsActive",
                schema: "Admin",
                table: "CertificateMetadata",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateMetadata_Kid",
                schema: "Admin",
                table: "CertificateMetadata",
                column: "Kid");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateMetadata_Thumbprint",
                schema: "Admin",
                table: "CertificateMetadata",
                column: "Thumbprint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertificateMetadata",
                schema: "Admin");
        }
    }
}
