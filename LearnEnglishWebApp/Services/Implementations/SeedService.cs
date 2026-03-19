using LearnEnglishWebApp.Data;
using LearnEnglishWebApp.Models;
using LearnEnglishWebApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearnEnglishWebApp.Services.Implementations
{
    public class SeedService : ISeedService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SeedService> _logger;

        public SeedService(AppDbContext context, ILogger<SeedService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAllDataAsync()
        {
            _logger.LogInformation("Начинаем заполнение данными...");

            try
            {
                await SeedDictionaryAsync();
                await SeedGrammarTopicsAsync();
                await SeedVocabLessonsAsync();
                await SeedVocabTestsAsync(); 

                _logger.LogInformation("Заполнение завершено");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при заполнении");
                throw;
            }
        }

        private async Task SeedDictionaryAsync()
        {
            if (await _context.Dictionaries.AnyAsync())
            {
                _logger.LogInformation("Словарь уже заполнен");
                return;
            }

            var words = new List<Dictionary>
            {
                new Dictionary
                {
                    Word = "hello",
                    Translation = "привет",
                    PartOfSpeech = "interjection",
                    DifficultyLevel = "A1",
                    Examples = new List<string> { "Hello, how are you?" },
                    CreatedAt = DateTime.UtcNow
                },
                new Dictionary
                {
                    Word = "goodbye",
                    Translation = "до свидания",
                    PartOfSpeech = "interjection",
                    DifficultyLevel = "A1",
                    Examples = new List<string> { "Goodbye, see you tomorrow!" },
                    CreatedAt = DateTime.UtcNow
                },
                new Dictionary
                {
                    Word = "cat",
                    Translation = "кот",
                    PartOfSpeech = "noun",
                    DifficultyLevel = "A1",
                    Examples = new List<string> { "The cat is sleeping" },
                    CreatedAt = DateTime.UtcNow
                },
                new Dictionary
                {
                    Word = "dog",
                    Translation = "собака",
                    PartOfSpeech = "noun",
                    DifficultyLevel = "A1",
                    Examples = new List<string> { "The dog is barking" },
                    CreatedAt = DateTime.UtcNow
                },
                new Dictionary
                {
                    Word = "work",
                    Translation = "работать",
                    PartOfSpeech = "verb",
                    DifficultyLevel = "A1",
                    Examples = new List<string> { "I work every day" },
                    CreatedAt = DateTime.UtcNow
                },
                new Dictionary
                {
                    Word = "study",
                    Translation = "учиться",
                    PartOfSpeech = "verb",
                    DifficultyLevel = "A1",
                    Examples = new List<string> { "She studies English" },
                    CreatedAt = DateTime.UtcNow
                }
            };

            foreach (var word in words)
            {
                var exists = await _context.Dictionaries
                    .AnyAsync(d => d.Word == word.Word);

                if (!exists)
                {
                    await _context.Dictionaries.AddAsync(word);
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Добавлено слов в словарь");
        }

        private async Task SeedGrammarTopicsAsync()
        {
            if (await _context.GrammarTopics.AnyAsync())
            {
                _logger.LogInformation("Грамматика уже заполнена");
                return;
            }

            var topics = new List<GrammarTopic>
            {
                new GrammarTopic
                {
                    Level = "A1",
                    Title = "Present Simple",
                    Description = "Настоящее простое время",
                    OrderIndex = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new GrammarTopic
                {
                    Level = "A1",
                    Title = "Past Simple",
                    Description = "Прошедшее простое время",
                    OrderIndex = 2,
                    CreatedAt = DateTime.UtcNow
                },
                new GrammarTopic
                {
                    Level = "A1",
                    Title = "Future Simple",
                    Description = "Будущее простое время",
                    OrderIndex = 3,
                    CreatedAt = DateTime.UtcNow
                }
            };

            foreach (var topic in topics)
            {
                var exists = await _context.GrammarTopics
                    .AnyAsync(t => t.Title == topic.Title && t.Level == topic.Level);

                if (!exists)
                {
                    await _context.GrammarTopics.AddAsync(topic);
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Добавлено грамматических тем");

            await SeedGrammarTestsAsync();
        }

        // 3. ТЕСТЫ по грамматике
        private async Task SeedGrammarTestsAsync()
        {
            if (await _context.GrammarTests.AnyAsync())
            {
                _logger.LogInformation("Грамматические тесты уже заполнены");
                return;
            }

            var presentSimple = await _context.GrammarTopics
                .FirstOrDefaultAsync(t => t.Title == "Present Simple");

            if (presentSimple != null)
            {
                var tests = new List<GrammarTest>
                {
                    new GrammarTest
                    {
                        GrammarTopicId = presentSimple.Id,
                        Title = "Present Simple - Тест 1",
                        Level = "A1",
                        QuestionCount = 5,
                        PassingScore = 60,
                        TimeLimitMinutes = 10,
                        OrderIndex = 1,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                foreach (var test in tests)
                {
                    var exists = await _context.GrammarTests
                        .AnyAsync(t => t.Title == test.Title && t.GrammarTopicId == test.GrammarTopicId);

                    if (!exists)
                    {
                        await _context.GrammarTests.AddAsync(test);
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Добавлено грамматических тестов");
            }
        }

        private async Task SeedVocabLessonsAsync()
        {
            if (await _context.VocabLessons.AnyAsync())
            {
                _logger.LogInformation("Vocab уроки уже заполнены");
                return;
            }

            var lessons = new List<VocabLesson>
            {
                new VocabLesson
                {
                    Level = "A1",
                    Title = "My Daily Routine",
                    Description = "Мой день - учим слова о повседневных делах",
                    OrderIndex = 1,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new VocabLesson
                {
                    Level = "A1",
                    Title = "My Family",
                    Description = "Моя семья - учим слова о семье",
                    OrderIndex = 2,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            };

            foreach (var lesson in lessons)
            {
                var exists = await _context.VocabLessons
                    .AnyAsync(l => l.Title == lesson.Title && l.Level == lesson.Level);

                if (!exists)
                {
                    await _context.VocabLessons.AddAsync(lesson);
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Добавлено {lessons.Count} Vocab уроков");
        }

        private async Task SeedVocabTestsAsync()
        {
            if (await _context.VocabTests.AnyAsync())
            {
                _logger.LogInformation("Vocab тесты уже заполнены");
                return;
            }

            var dailyRoutine = await _context.VocabLessons
                .FirstOrDefaultAsync(l => l.Title == "My Daily Routine");

            var myFamily = await _context.VocabLessons
                .FirstOrDefaultAsync(l => l.Title == "My Family");

            var tests = new List<VocabTest>();

            if (dailyRoutine != null)
            {
                tests.Add(new VocabTest
                {
                    VocabLessonId = dailyRoutine.Id,
                    Title = "Daily Routine - Comprehension",
                    Level = "A1",
                    Description = "Проверьте понимание текста о повседневных делах",
                    TestType = "comprehension",
                    QuestionCount = 5,
                    PassingScore = 60,
                    TimeLimitMinutes = 10,
                    OrderIndex = 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });

                tests.Add(new VocabTest
                {
                    VocabLessonId = dailyRoutine.Id,
                    Title = "Daily Routine - Vocabulary",
                    Level = "A1",
                    Description = "Проверьте знание слов из урока",
                    TestType = "vocabulary",
                    QuestionCount = 8,
                    PassingScore = 70,
                    TimeLimitMinutes = 15,
                    OrderIndex = 2,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (myFamily != null)
            {
                tests.Add(new VocabTest
                {
                    VocabLessonId = myFamily.Id,
                    Title = "Family - Comprehension",
                    Level = "A1",
                    Description = "Проверьте понимание текста о семье",
                    TestType = "comprehension",
                    QuestionCount = 5,
                    PassingScore = 60,
                    TimeLimitMinutes = 10,
                    OrderIndex = 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });

                tests.Add(new VocabTest
                {
                    VocabLessonId = myFamily.Id,
                    Title = "Family - Vocabulary",
                    Level = "A1",
                    Description = "Проверьте знание слов о семье",
                    TestType = "vocabulary",
                    QuestionCount = 8,
                    PassingScore = 70,
                    TimeLimitMinutes = 15,
                    OrderIndex = 2,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            foreach (var test in tests)
            {
                var exists = await _context.VocabTests
                    .AnyAsync(t => t.Title == test.Title && t.VocabLessonId == test.VocabLessonId);

                if (!exists)
                {
                    await _context.VocabTests.AddAsync(test);
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Добавлено {tests.Count} Vocab тестов");
        }
    }
}