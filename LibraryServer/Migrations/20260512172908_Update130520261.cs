using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryServer.Migrations
{
    /// <inheritdoc />
    public partial class Update130520261 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forums_Books_BookId",
                table: "Forums");

            migrationBuilder.AlterColumn<int>(
                name: "BookId",
                table: "Forums",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

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

            migrationBuilder.AlterColumn<int>(
                name: "BookId",
                table: "Forums",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Forums_Books_BookId",
                table: "Forums",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
