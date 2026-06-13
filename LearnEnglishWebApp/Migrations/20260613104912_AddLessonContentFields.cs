using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnEnglishWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonContentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExamplesContent",
                table: "VocabLessons",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TheoryContent",
                table: "VocabLessons",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamplesContent",
                table: "VocabLessons");

            migrationBuilder.DropColumn(
                name: "TheoryContent",
                table: "VocabLessons");
        }
    }
}
