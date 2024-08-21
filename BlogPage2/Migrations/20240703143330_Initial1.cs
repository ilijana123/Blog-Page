using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogPage2.Migrations
{
    /// <inheritdoc />
    public partial class Initial1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Comment");

            migrationBuilder.CreateIndex(
                name: "IX_Like_PostId",
                table: "Like",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Like_Post_PostId",
                table: "Like",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Like_Post_PostId",
                table: "Like");

            migrationBuilder.DropIndex(
                name: "IX_Like_PostId",
                table: "Like");

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Comment",
                type: "int",
                nullable: true);
        }
    }
}
