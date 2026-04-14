using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryServer.Migrations
{
    /// <inheritdoc />
    public partial class Update14042026EventsPhotoModel1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventPhoto_Events_EventsId",
                table: "EventPhoto");

            migrationBuilder.DropIndex(
                name: "IX_EventPhoto_EventsId",
                table: "EventPhoto");

            migrationBuilder.DropColumn(
                name: "EventsId",
                table: "EventPhoto");

            migrationBuilder.CreateIndex(
                name: "IX_EventPhoto_EventId",
                table: "EventPhoto",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventPhoto_Events_EventId",
                table: "EventPhoto",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventPhoto_Events_EventId",
                table: "EventPhoto");

            migrationBuilder.DropIndex(
                name: "IX_EventPhoto_EventId",
                table: "EventPhoto");

            migrationBuilder.AddColumn<int>(
                name: "EventsId",
                table: "EventPhoto",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EventPhoto_EventsId",
                table: "EventPhoto",
                column: "EventsId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventPhoto_Events_EventsId",
                table: "EventPhoto",
                column: "EventsId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
