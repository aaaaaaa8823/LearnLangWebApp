using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnEnglishWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddGrammarContentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExamplesContent",
                table: "GrammarTopics",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TheoryContent",
                table: "GrammarTopics",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamplesContent",
                table: "GrammarTopics");

            migrationBuilder.DropColumn(
                name: "TheoryContent",
                table: "GrammarTopics");
        }
    }
}
