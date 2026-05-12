using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryServer.Migrations
{
    /// <inheritdoc />
    public partial class Update13052026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationPath",
                table: "ForumMessages");

            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "Forums",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Forums_BookId",
                table: "Forums",
                column: "BookId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Forums_Books_BookId",
                table: "Forums",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forums_Books_BookId",
                table: "Forums");

            migrationBuilder.DropIndex(
                name: "IX_Forums_BookId",
                table: "Forums");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "Forums");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationPath",
                table: "ForumMessages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
