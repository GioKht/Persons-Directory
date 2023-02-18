using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persons.Directory.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoverRedundantImgProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Persons");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
