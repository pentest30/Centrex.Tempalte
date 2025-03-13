using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saylo.Centrex.Identity.Core.Migrations
{
    /// <inheritdoc />
    public partial class userInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Admin",
                table: "ServiceProviders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                schema: "Admin",
                table: "ServiceProviders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateBy",
                schema: "Admin",
                table: "ServiceProviders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateById",
                schema: "Admin",
                table: "ServiceProviders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Admin",
                table: "OutboxEvent",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                schema: "Admin",
                table: "OutboxEvent",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateBy",
                schema: "Admin",
                table: "OutboxEvent",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateById",
                schema: "Admin",
                table: "OutboxEvent",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Admin",
                table: "Modules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                schema: "Admin",
                table: "Modules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateBy",
                schema: "Admin",
                table: "Modules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateById",
                schema: "Admin",
                table: "Modules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Admin",
                table: "Functionalities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                schema: "Admin",
                table: "Functionalities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateBy",
                schema: "Admin",
                table: "Functionalities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateById",
                schema: "Admin",
                table: "Functionalities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Admin",
                table: "Enterprises",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                schema: "Admin",
                table: "Enterprises",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateBy",
                schema: "Admin",
                table: "Enterprises",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateById",
                schema: "Admin",
                table: "Enterprises",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Admin",
                table: "AdministrationDomains",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                schema: "Admin",
                table: "AdministrationDomains",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateBy",
                schema: "Admin",
                table: "AdministrationDomains",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateById",
                schema: "Admin",
                table: "AdministrationDomains",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "Admin",
                table: "AdministrationDomains",
                keyColumn: "Id",
                keyValue: new Guid("bdcb855f-a6a0-444f-997e-0004603d6c93"),
                columns: new[] { "CreatedBy", "CreatedById", "UpdateBy", "UpdateById" },
                values: new object[] { null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Admin",
                table: "ServiceProviders");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                schema: "Admin",
                table: "ServiceProviders");

            migrationBuilder.DropColumn(
                name: "UpdateBy",
                schema: "Admin",
                table: "ServiceProviders");

            migrationBuilder.DropColumn(
                name: "UpdateById",
                schema: "Admin",
                table: "ServiceProviders");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Admin",
                table: "OutboxEvent");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                schema: "Admin",
                table: "OutboxEvent");

            migrationBuilder.DropColumn(
                name: "UpdateBy",
                schema: "Admin",
                table: "OutboxEvent");

            migrationBuilder.DropColumn(
                name: "UpdateById",
                schema: "Admin",
                table: "OutboxEvent");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Admin",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                schema: "Admin",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "UpdateBy",
                schema: "Admin",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "UpdateById",
                schema: "Admin",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Admin",
                table: "Functionalities");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                schema: "Admin",
                table: "Functionalities");

            migrationBuilder.DropColumn(
                name: "UpdateBy",
                schema: "Admin",
                table: "Functionalities");

            migrationBuilder.DropColumn(
                name: "UpdateById",
                schema: "Admin",
                table: "Functionalities");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Admin",
                table: "Enterprises");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                schema: "Admin",
                table: "Enterprises");

            migrationBuilder.DropColumn(
                name: "UpdateBy",
                schema: "Admin",
                table: "Enterprises");

            migrationBuilder.DropColumn(
                name: "UpdateById",
                schema: "Admin",
                table: "Enterprises");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Admin",
                table: "AdministrationDomains");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                schema: "Admin",
                table: "AdministrationDomains");

            migrationBuilder.DropColumn(
                name: "UpdateBy",
                schema: "Admin",
                table: "AdministrationDomains");

            migrationBuilder.DropColumn(
                name: "UpdateById",
                schema: "Admin",
                table: "AdministrationDomains");
        }
    }
}
