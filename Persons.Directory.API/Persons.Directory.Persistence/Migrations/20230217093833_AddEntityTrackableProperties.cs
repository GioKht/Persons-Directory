using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persons.Directory.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityTrackableProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Persons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDate",
                table: "Persons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Persons",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "DeleteDate",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Persons");
        }
    }
}
