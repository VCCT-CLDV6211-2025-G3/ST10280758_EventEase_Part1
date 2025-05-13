using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventEase.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Venue_VenueId",
                table: "Event");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Event",
                newName: "ImageUrl");

            migrationBuilder.AlterColumn<int>(
                name: "VenueId",
                table: "Event",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EventName",
                table: "Event",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Venue_VenueId",
                table: "Event",
                column: "VenueId",
                principalTable: "Venue",
                principalColumn: "VenueId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Venue_VenueId",
                table: "Event");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Event",
                newName: "Description");

            migrationBuilder.AlterColumn<int>(
                name: "VenueId",
                table: "Event",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "EventName",
                table: "Event",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Venue_VenueId",
                table: "Event",
                column: "VenueId",
                principalTable: "Venue",
                principalColumn: "VenueId");
        }
    }
}
