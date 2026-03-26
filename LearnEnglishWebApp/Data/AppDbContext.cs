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

        public DbSet<GrammarTest> GrammarTests { get; set; }
        public DbSet<VocabTest> VocabTests { get; set; }

        public DbSet<TestResult> TestResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Существующие составные ключи
            modelBuilder.Entity<CollectionWord>().HasKey(cw => new { cw.CollectionId, cw.UserWordId });
            modelBuilder.Entity<UserGrammarProgress>().HasKey(ugp => new { ugp.UserId, ugp.GrammarTopicId });
            modelBuilder.Entity<UserVocabProgress>().HasKey(uvp => new { uvp.UserId, uvp.LessonId });

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

    }
}