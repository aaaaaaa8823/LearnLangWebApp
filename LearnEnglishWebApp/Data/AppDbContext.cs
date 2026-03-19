using Microsoft.EntityFrameworkCore;
using LearnEnglishWebApp.Models;

namespace LearnEnglishWebApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Dictionary> Dictionaries { get; set; }
        public DbSet<UserWord> UsersWords { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<CollectionWord> CollectionsWords { get; set; }
        public DbSet<GrammarTopic> GrammarTopics { get; set; }
        public DbSet<UserGrammarProgress> UserGrammarProgress { get; set; }
        public DbSet<VocabLesson> VocabLessons { get; set; }
        public DbSet<UserVocabProgress> UserVocabProgresses { get; set; }

        // Новые DbSet для тестов (без JSON)
        public DbSet<GrammarTest> GrammarTests { get; set; }
        public DbSet<VocabTest> VocabTests { get; set; }

        // УНИВЕРСАЛЬНАЯ таблица для результатов тестов
        public DbSet<TestResult> TestResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Существующие составные ключи
            modelBuilder.Entity<CollectionWord>().HasKey(cw => new { cw.CollectionId, cw.UserWordId });
            modelBuilder.Entity<UserGrammarProgress>().HasKey(ugp => new { ugp.UserId, ugp.GrammarTopicId });
            modelBuilder.Entity<UserVocabProgress>().HasKey(uvp => new { uvp.UserId, uvp.LessonId });

            // Новый составной ключ для TestResult (уникальность попытки)
            modelBuilder.Entity<TestResult>()
                .HasKey(tr => new { tr.UserId, tr.TestType, tr.TestId, tr.AttemptNumber });

            // Уникальные индексы
            modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Dictionary>().HasIndex(d => d.Word).IsUnique();
            modelBuilder.Entity<UserWord>().HasIndex(uw => new { uw.UserId, uw.WordId }).IsUnique();
            modelBuilder.Entity<Collection>().HasIndex(c => new { c.UserId, c.Name }).IsUnique();

            // Связи для UserWord
            modelBuilder.Entity<UserWord>()
                .HasOne(uw => uw.User)
                .WithMany(u => u.UserWords)
                .HasForeignKey(uw => uw.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserWord>()
                .HasOne(uw => uw.Word)
                .WithMany(d => d.UserWords)
                .HasForeignKey(uw => uw.WordId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связи для CollectionWord
            modelBuilder.Entity<CollectionWord>()
                .HasOne(cw => cw.Collection)
                .WithMany(c => c.CollectionWords)
                .HasForeignKey(cw => cw.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CollectionWord>()
                .HasOne(cw => cw.UserWord)
                .WithMany(uw => uw.CollectionWords)
                .HasForeignKey(cw => cw.UserWordId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связи для GrammarTest
            modelBuilder.Entity<GrammarTest>()
                .HasOne(gt => gt.GrammarTopic)
                .WithMany(g => g.GrammarTests)
                .HasForeignKey(gt => gt.GrammarTopicId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связи для VocabTest
            modelBuilder.Entity<VocabTest>()
                .HasOne(vt => vt.VocabLesson)
                .WithMany(v => v.VocabTests)
                .HasForeignKey(vt => vt.VocabLessonId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связи для TestResult
            modelBuilder.Entity<TestResult>()
                .HasOne(tr => tr.User)
                .WithMany(u => u.TestResults)
                .HasForeignKey(tr => tr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Индексы для производительности
            modelBuilder.Entity<UserWord>().HasIndex(uw => uw.Status);
            modelBuilder.Entity<Dictionary>().HasIndex(d => d.DifficultyLevel);

            // Индексы для тестов
            modelBuilder.Entity<GrammarTest>().HasIndex(gt => new { gt.Level, gt.OrderIndex });
            modelBuilder.Entity<VocabTest>().HasIndex(vt => new { vt.Level, vt.OrderIndex });
            modelBuilder.Entity<GrammarTopic>().HasIndex(gt => new { gt.Level, gt.OrderIndex });
            modelBuilder.Entity<VocabLesson>().HasIndex(vl => new { vl.Level, vl.OrderIndex });

            // Индексы для TestResult
            modelBuilder.Entity<TestResult>().HasIndex(tr => tr.CompletedAt);
            modelBuilder.Entity<TestResult>().HasIndex(tr => tr.TestType);
            modelBuilder.Entity<TestResult>().HasIndex(tr => new { tr.TestType, tr.TestId });
            modelBuilder.Entity<TestResult>().HasIndex(tr => tr.IsPassed);
        }

        //public static void SeedData(AppDbContext context)
        //{
        //    if (!context.GrammarTopics.Any())
        //    {
        //        context.GrammarTopics.AddRange(
        //            new GrammarTopic
        //            {
        //                Level = "A1",
        //                Title = "Present Simple",
        //                Description = "Настоящее простое время",
        //                OrderIndex = 1
        //            },
        //            new GrammarTopic
        //            {
        //                Level = "A1",
        //                Title = "Past Simple",
        //                Description = "Прошедшее простое время",
        //                OrderIndex = 2
        //            },
        //            new GrammarTopic
        //            {
        //                Level = "A1",
        //                Title = "Future Simple",
        //                Description = "Будущее простое время",
        //                OrderIndex = 3
        //            }
        //        );
        //        context.SaveChanges();
        //    }

        //    if (!context.GrammarTests.Any() && context.GrammarTopics.Any())
        //    {
        //        var presentSimpleTopic = context.GrammarTopics
        //            .FirstOrDefault(gt => gt.Title == "Present Simple");

        //        if (presentSimpleTopic != null)
        //        {
        //            context.GrammarTests.AddRange(
        //                new GrammarTest
        //                {
        //                    GrammarTopicId = presentSimpleTopic.Id,
        //                    Title = "Present Simple - Basic Test",
        //                    Level = "A1",
        //                    QuestionCount = 10,
        //                    PassingScore = 70,
        //                    TimeLimitMinutes = 15,
        //                    OrderIndex = 1
        //                },
        //                new GrammarTest
        //                {
        //                    GrammarTopicId = presentSimpleTopic.Id,
        //                    Title = "Present Simple - Advanced Test",
        //                    Level = "A1",
        //                    QuestionCount = 15,
        //                    PassingScore = 80,
        //                    TimeLimitMinutes = 20,
        //                    OrderIndex = 2
        //                }
        //            );
        //            context.SaveChanges();
        //        }
        //    }

        //    // Добавляем Vocab уроки
        //    if (!context.VocabLessons.Any())
        //    {
        //        context.VocabLessons.AddRange(
        //            new VocabLesson
        //            {
        //                Level = "A1",
        //                Title = "My Daily Routine",
        //                Description = "Изучите слова о повседневных делах",
        //                ContentKey = "a1-my-daily-routine",
        //                VocabularyWordIds = new List<long>(), // Здесь будут ID слов
        //                OrderIndex = 1
        //            },
        //            new VocabLesson
        //            {
        //                Level = "A1",
        //                Title = "My Family",
        //                Description = "Слова о семье",
        //                ContentKey = "a1-my-family",
        //                VocabularyWordIds = new List<long>(),
        //                OrderIndex = 2
        //            }
        //        );
        //        context.SaveChanges();
        //    }

        //    // Добавляем тесты для Vocab
        //    if (!context.VocabTests.Any() && context.VocabLessons.Any())
        //    {
        //        var dailyRoutineLesson = context.VocabLessons
        //            .FirstOrDefault(vl => vl.Title == "My Daily Routine");

        //        if (dailyRoutineLesson != null)
        //        {
        //            context.VocabTests.AddRange(
        //                new VocabTest
        //                {
        //                    VocabLessonId = dailyRoutineLesson.Id,
        //                    Title = "Daily Routine - Comprehension Test",
        //                    Description = "Проверьте понимание текста",
        //                    Level = "A1",
        //                    TestKey = "a1-daily-routine-comprehension",
        //                    TestType = "comprehension",
        //                    QuestionCount = 8,
        //                    PassingScore = 70,
        //                    TimeLimitMinutes = 10,
        //                    OrderIndex = 1
        //                },
        //                new VocabTest
        //                {
        //                    VocabLessonId = dailyRoutineLesson.Id,
        //                    Title = "Daily Routine - Vocabulary Test",
        //                    Description = "Проверьте знание слов из текста",
        //                    Level = "A1",
        //                    TestKey = "a1-daily-routine-vocabulary",
        //                    TestType = "vocabulary",
        //                    QuestionCount = 12,
        //                    PassingScore = 75,
        //                    TimeLimitMinutes = 12,
        //                    OrderIndex = 2
        //                }
        //            );
        //            context.SaveChanges();
        //        }
        //    }

        //    // Добавляем слова в словарь
        //    if (!context.Dictionaries.Any())
        //    {
        //        context.Dictionaries.AddRange(
        //            new Dictionary
        //            {
        //                Word = "hello",
        //                Translation = "привет",
        //                PartOfSpeech = "interjection",
        //                DifficultyLevel = "A1",
        //                Examples = new List<string> { "Hello, how are you?" }
        //            },
        //            new Dictionary
        //            {
        //                Word = "world",
        //                Translation = "мир",
        //                PartOfSpeech = "noun",
        //                DifficultyLevel = "A1",
        //                Examples = new List<string> { "The whole world" }
        //            },
        //            new Dictionary
        //            {
        //                Word = "wake up",
        //                Translation = "просыпаться",
        //                PartOfSpeech = "verb",
        //                DifficultyLevel = "A1",
        //                Examples = new List<string> { "I wake up at 7 AM" }
        //            },
        //            new Dictionary
        //            {
        //                Word = "breakfast",
        //                Translation = "завтрак",
        //                PartOfSpeech = "noun",
        //                DifficultyLevel = "A1",
        //                Examples = new List<string> { "I eat breakfast at 8 AM" }
        //            }
        //        );
        //        context.SaveChanges();
        //    }
        //}
    }
}