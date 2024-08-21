using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogPage2.Migrations
{
    /// <inheritdoc />
    public partial class PostAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUser",
                table: "Post",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppUser",
                table: "Post");
        }
    }
}
