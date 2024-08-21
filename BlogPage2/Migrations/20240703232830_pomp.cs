using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogPage2.Migrations
{
    /// <inheritdoc />
    public partial class pomp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserPId",
                table: "Post",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_UserPId",
                table: "Post",
                column: "UserPId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_PostId",
                table: "Comment",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Post_PostId",
                table: "Comment",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_UserP_UserPId",
                table: "Post",
                column: "UserPId",
                principalTable: "UserP",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Post_PostId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_UserP_UserPId",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_UserPId",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Comment_PostId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "UserPId",
                table: "Post");
        }
    }
}
