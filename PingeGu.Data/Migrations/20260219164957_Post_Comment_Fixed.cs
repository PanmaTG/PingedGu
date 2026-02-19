using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PingedGu.Migrations
{
    /// <inheritdoc />
    public partial class Post_Comment_Fixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateUploaded",
                table: "Comments",
                newName: "DateUpdated");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateUpdated",
                table: "Comments",
                newName: "DateUploaded");
        }
    }
}
