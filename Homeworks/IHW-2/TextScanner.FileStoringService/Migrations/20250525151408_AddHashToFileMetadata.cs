using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TextScanner.FileStoringService.Migrations
{
    /// <inheritdoc />
    public partial class AddHashToFileMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "FileMetadatas",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hash",
                table: "FileMetadatas");
        }
    }
}
