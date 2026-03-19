using System;
using System.Collections.Generic;
using LearnEnglishWebApp.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnEnglishWebApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVocabLessons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VocabularyWordIds",
                table: "VocabLessons");

            migrationBuilder.DropColumn(
                name: "AnswerDetails",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "GrammarTopics");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VocabularyWordIds",
                table: "VocabLessons",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<List<TestAnswerDetail>>(
                name: "AnswerDetails",
                table: "TestResults",
                type: "jsonb",
                nullable: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "GrammarTopics",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
