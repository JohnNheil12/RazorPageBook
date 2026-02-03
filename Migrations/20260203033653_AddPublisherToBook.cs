using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazorPageBooks.Migrations
{
    /// <inheritdoc />
    public partial class AddPublisherToBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Publisher",
                table: "Book",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Publisher",
                table: "Book");
        }
    }
}
