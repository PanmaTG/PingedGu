using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PingedGu.Migrations
{
    /// <inheritdoc />
    public partial class Rename_Notifications_Date_Columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Updated",
                table: "Notifications",
                newName: "DateUpdated");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "Notifications",
                newName: "DateCreated");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateUpdated",
                table: "Notifications",
                newName: "Updated");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "Notifications",
                newName: "Created");
        }
    }
}
