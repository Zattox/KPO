using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TextScanner.FileAnalysisService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAnalysisResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AnalysisResults",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "IsPlagiarized",
                table: "AnalysisResults");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnalysisResults",
                table: "AnalysisResults",
                column: "FileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AnalysisResults",
                table: "AnalysisResults");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AnalysisResults",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<bool>(
                name: "IsPlagiarized",
                table: "AnalysisResults",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnalysisResults",
                table: "AnalysisResults",
                column: "Id");
        }
    }
}
