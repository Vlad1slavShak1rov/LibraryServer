using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryServer.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherStudentToAssignedTes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedTest_Users_UserId",
                table: "AssignedTest");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AssignedTest",
                newName: "TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_AssignedTest_UserId",
                table: "AssignedTest",
                newName: "IX_AssignedTest_TeacherId");

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "AssignedTest",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AssignedTest_StudentId",
                table: "AssignedTest",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedTest_Users_StudentId",
                table: "AssignedTest",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedTest_Users_TeacherId",
                table: "AssignedTest",
                column: "TeacherId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedTest_Users_StudentId",
                table: "AssignedTest");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignedTest_Users_TeacherId",
                table: "AssignedTest");

            migrationBuilder.DropIndex(
                name: "IX_AssignedTest_StudentId",
                table: "AssignedTest");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "AssignedTest");

            migrationBuilder.RenameColumn(
                name: "TeacherId",
                table: "AssignedTest",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AssignedTest_TeacherId",
                table: "AssignedTest",
                newName: "IX_AssignedTest_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedTest_Users_UserId",
                table: "AssignedTest",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
