using Microsoft.EntityFrameworkCore;
using LearnEnglishWebApp.Models;

namespace LearnEnglishWebApp.Data
{
    public class AppDbContext: DbContext
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //первичные ключи
            modelBuilder.Entity<CollectionWord>().HasKey(cw => new { cw.CollectionId, cw.UserWordId });
            modelBuilder.Entity<UserGrammarProgress>().HasKey(ugp => new { ugp.UserId, ugp.GrammarTopicId });
            modelBuilder.Entity<UserVocabProgress>().HasKey(uvp => new { uvp.UserId, uvp.LessonId });

            //уникальные значения
            modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Dictionary>().HasIndex(d => d.Word).IsUnique();
            modelBuilder.Entity<UserWord>().HasIndex(uw => new { uw.UserId, uw.WordId }).IsUnique();
            modelBuilder.Entity<Collection>().HasIndex(c => new { c.UserId, c.Name }).IsUnique();

            //связи
            //для UserWord
            modelBuilder.Entity<UserWord>().HasOne(uw => uw.User).WithMany(u => u.UserWords).HasForeignKey(uw => uw.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserWord>().HasOne(uw => uw.Word).WithMany(u => u.UserWords).HasForeignKey(uw => uw.WordId).OnDelete(DeleteBehavior.Cascade);

            //для CollectionWord
            modelBuilder.Entity<CollectionWord>().HasOne(cw => cw.Collection).WithMany(c => c.CollectionWords).HasForeignKey(cw => cw.CollectionId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<CollectionWord>().HasOne(cw => cw.UserWord).WithMany(uw => uw.CollectionWords).HasForeignKey(cw => cw.UserWordId).OnDelete(DeleteBehavior.Cascade);

            // Индексы для производительности
            modelBuilder.Entity<UserWord>().HasIndex(uw => uw.Status);

            //modelBuilder.Entity<UserWord>()
            //    .HasIndex(uw => uw.LastReviewed);

            modelBuilder.Entity<Dictionary>().HasIndex(d => d.DifficultyLevel);

            // Настройка JSONB полей
            modelBuilder.Entity<Dictionary>().Property(d => d.Examples).HasColumnType("jsonb");

            modelBuilder.Entity<GrammarTopic>().Property(g => g.Examples).HasColumnType("jsonb");

            //modelBuilder.Entity<VocabLesson>().Property(v => v.Questions).HasColumnType("jsonb");
        }

        public static void SeedData(AppDbContext context)
            {
            if (!context.GrammarTopics.Any())
            {
                context.GrammarTopics.AddRange(
                    new GrammarTopic
                    {
                        Level = "A1",
                        Title = "Present Simple",
                        Description = "Настоящее простое время",
                        OrderIndex = 1,
                        Examples = new List<string>
                        {
                            "I work every day",
                            "She reads books"
                        }
                    },
                    new GrammarTopic
                    {
                        Level = "A1",
                        Title = "Past Simple",
                        Description = "Прошедшее простое время",
                        OrderIndex = 2,
                        Examples = new List<string>
                        {
                            "I worked yesterday",
                            "She read a book"
                        }
                    }
                );
            }

            if (!context.Dictionaries.Any())
            {
                context.Dictionaries.AddRange(
                    new Dictionary
                    {
                        Word = "hello",
                        Translation = "привет",
                        PartOfSpeech = "interjection",
                        DifficultyLevel = "A1",
                        Examples = new List<string> { "Hello, how are you?" }
                    },
                    new Dictionary
                    {
                        Word = "world",
                        Translation = "мир",
                        PartOfSpeech = "noun",
                        DifficultyLevel = "A1",
                        Examples = new List<string> { "The whole world" }
                    }
                );
            }

            context.SaveChanges();
        }

    }
}
