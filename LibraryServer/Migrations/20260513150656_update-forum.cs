using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryServer.Migrations
{
    /// <inheritdoc />
    public partial class updateforum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "Forums",
                type: "INTEGER",
                nullable: true);

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
                principalColumn: "Id");
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
        }
    }
}
