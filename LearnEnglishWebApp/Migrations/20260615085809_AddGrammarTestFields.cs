using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnEnglishWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddGrammarTestFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnswersText",
                table: "GrammarTests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestionsText",
                table: "GrammarTests",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswersText",
                table: "GrammarTests");

            migrationBuilder.DropColumn(
                name: "QuestionsText",
                table: "GrammarTests");
        }
    }
}
