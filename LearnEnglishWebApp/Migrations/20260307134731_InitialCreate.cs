using System;
using System.Collections.Generic;
using LearnEnglishWebApp.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LearnEnglishWebApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dictionaries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Word = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Translation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Definition = table.Column<string>(type: "text", nullable: true),
                    PartOfSpeech = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DifficultyLevel = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Examples = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictionaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GrammarTopics",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Level = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ContentKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrammarTopics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(225)", maxLength: 225, nullable: false),
                    Level = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VocabLessons",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Level = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ContentKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    VocabularyWordIds = table.Column<string>(type: "jsonb", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabLessons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GrammarTests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GrammarTopicId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Level = table.Column<string>(type: "text", nullable: false),
                    QuestionCount = table.Column<int>(type: "integer", nullable: false),
                    TestKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PassingScore = table.Column<int>(type: "integer", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    TimeLimitMinutes = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrammarTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrammarTests_GrammarTopics_GrammarTopicId",
                        column: x => x.GrammarTopicId,
                        principalTable: "GrammarTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Collections_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestResults",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TestType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TestId = table.Column<long>(type: "bigint", nullable: false),
                    AttemptNumber = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    MaxScore = table.Column<int>(type: "integer", nullable: false),
                    Percentage = table.Column<double>(type: "double precision", nullable: false),
                    IsPassed = table.Column<bool>(type: "boolean", nullable: false),
                    TimeSpentSeconds = table.Column<int>(type: "integer", nullable: false),
                    CorrectAnswers = table.Column<int>(type: "integer", nullable: false),
                    WrongAnswers = table.Column<int>(type: "integer", nullable: false),
                    AnswerDetails = table.Column<List<TestAnswerDetail>>(type: "jsonb", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResults", x => new { x.UserId, x.TestType, x.TestId, x.AttemptNumber });
                    table.ForeignKey(
                        name: "FK_TestResults_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGrammarProgress",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    GrammarTopicId = table.Column<long>(type: "bigint", nullable: false),
                    Completed = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGrammarProgress", x => new { x.UserId, x.GrammarTopicId });
                    table.ForeignKey(
                        name: "FK_UserGrammarProgress_GrammarTopics_GrammarTopicId",
                        column: x => x.GrammarTopicId,
                        principalTable: "GrammarTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGrammarProgress_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersWords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    WordId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ContextSentence = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersWords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersWords_Dictionaries_WordId",
                        column: x => x.WordId,
                        principalTable: "Dictionaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersWords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserVocabProgresses",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LessonId = table.Column<long>(type: "bigint", nullable: false),
                    Completed = table.Column<bool>(type: "boolean", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVocabProgresses", x => new { x.UserId, x.LessonId });
                    table.ForeignKey(
                        name: "FK_UserVocabProgresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserVocabProgresses_VocabLessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "VocabLessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VocabTests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VocabLessonId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Level = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TestKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TestType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    QuestionCount = table.Column<int>(type: "integer", nullable: false),
                    PassingScore = table.Column<int>(type: "integer", nullable: false),
                    TimeLimitMinutes = table.Column<int>(type: "integer", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabTests_VocabLessons_VocabLessonId",
                        column: x => x.VocabLessonId,
                        principalTable: "VocabLessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectionsWords",
                columns: table => new
                {
                    CollectionId = table.Column<long>(type: "bigint", nullable: false),
                    UserWordId = table.Column<long>(type: "bigint", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionsWords", x => new { x.CollectionId, x.UserWordId });
                    table.ForeignKey(
                        name: "FK_CollectionsWords_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionsWords_UsersWords_UserWordId",
                        column: x => x.UserWordId,
                        principalTable: "UsersWords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Collections_UserId_Name",
                table: "Collections",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollectionsWords_UserWordId",
                table: "CollectionsWords",
                column: "UserWordId");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_DifficultyLevel",
                table: "Dictionaries",
                column: "DifficultyLevel");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_Word",
                table: "Dictionaries",
                column: "Word",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GrammarTests_GrammarTopicId",
                table: "GrammarTests",
                column: "GrammarTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_GrammarTests_Level_OrderIndex",
                table: "GrammarTests",
                columns: new[] { "Level", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_GrammarTests_TestKey",
                table: "GrammarTests",
                column: "TestKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GrammarTopics_ContentKey",
                table: "GrammarTopics",
                column: "ContentKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GrammarTopics_Level_OrderIndex",
                table: "GrammarTopics",
                columns: new[] { "Level", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_CompletedAt",
                table: "TestResults",
                column: "CompletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_IsPassed",
                table: "TestResults",
                column: "IsPassed");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_TestType",
                table: "TestResults",
                column: "TestType");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_TestType_TestId",
                table: "TestResults",
                columns: new[] { "TestType", "TestId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserGrammarProgress_GrammarTopicId",
                table: "UserGrammarProgress",
                column: "GrammarTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersWords_Status",
                table: "UsersWords",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_UsersWords_UserId_WordId",
                table: "UsersWords",
                columns: new[] { "UserId", "WordId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersWords_WordId",
                table: "UsersWords",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVocabProgresses_LessonId",
                table: "UserVocabProgresses",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabLessons_ContentKey",
                table: "VocabLessons",
                column: "ContentKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VocabLessons_Level_OrderIndex",
                table: "VocabLessons",
                columns: new[] { "Level", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_VocabTests_Level_OrderIndex",
                table: "VocabTests",
                columns: new[] { "Level", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_VocabTests_TestKey",
                table: "VocabTests",
                column: "TestKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VocabTests_VocabLessonId",
                table: "VocabTests",
                column: "VocabLessonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectionsWords");

            migrationBuilder.DropTable(
                name: "GrammarTests");

            migrationBuilder.DropTable(
                name: "TestResults");

            migrationBuilder.DropTable(
                name: "UserGrammarProgress");

            migrationBuilder.DropTable(
                name: "UserVocabProgresses");

            migrationBuilder.DropTable(
                name: "VocabTests");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "UsersWords");

            migrationBuilder.DropTable(
                name: "GrammarTopics");

            migrationBuilder.DropTable(
                name: "VocabLessons");

            migrationBuilder.DropTable(
                name: "Dictionaries");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
