using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogPage2.Migrations
{
    /// <inheritdoc />
    public partial class nova1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_UserP_UserPId",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_UserPId",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "UserPId",
                table: "Post");

            migrationBuilder.CreateIndex(
                name: "IX_UserP_PostId",
                table: "UserP",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserP_Post_PostId",
                table: "UserP",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserP_Post_PostId",
                table: "UserP");

            migrationBuilder.DropIndex(
                name: "IX_UserP_PostId",
                table: "UserP");

            migrationBuilder.AddColumn<int>(
                name: "UserPId",
                table: "Post",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_UserPId",
                table: "Post",
                column: "UserPId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_UserP_UserPId",
                table: "Post",
                column: "UserPId",
                principalTable: "UserP",
                principalColumn: "Id");
        }
    }
}
