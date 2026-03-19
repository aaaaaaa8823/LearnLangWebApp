using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnEnglishWebApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VocabTests_TestKey",
                table: "VocabTests");

            migrationBuilder.DropIndex(
                name: "IX_VocabLessons_ContentKey",
                table: "VocabLessons");

            migrationBuilder.DropIndex(
                name: "IX_GrammarTopics_ContentKey",
                table: "GrammarTopics");

            migrationBuilder.DropIndex(
                name: "IX_GrammarTests_TestKey",
                table: "GrammarTests");

            migrationBuilder.DropColumn(
                name: "TestKey",
                table: "VocabTests");

            migrationBuilder.DropColumn(
                name: "ContentKey",
                table: "VocabLessons");

            migrationBuilder.DropColumn(
                name: "ContentKey",
                table: "GrammarTopics");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "GrammarTopics");

            migrationBuilder.DropColumn(
                name: "TestKey",
                table: "GrammarTests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TestKey",
                table: "VocabTests",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentKey",
                table: "VocabLessons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentKey",
                table: "GrammarTopics",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "GrammarTopics",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TestKey",
                table: "GrammarTests",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_VocabTests_TestKey",
                table: "VocabTests",
                column: "TestKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VocabLessons_ContentKey",
                table: "VocabLessons",
                column: "ContentKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GrammarTopics_ContentKey",
                table: "GrammarTopics",
                column: "ContentKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GrammarTests_TestKey",
                table: "GrammarTests",
                column: "TestKey",
                unique: true);
        }
    }
}
