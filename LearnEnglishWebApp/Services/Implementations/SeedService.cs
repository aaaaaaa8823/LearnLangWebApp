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
                await SeedAdminUserAsync();
                await SeedDictionaryAsync();
                await SeedGrammarTopicsAsync();
                await SeedVocabLessonsAsync();
                await SeedVocabTestsAsync();
                await UpdateGrammarTopicsContentAsync();
                await UpdateVocabLessonsContentAsync();

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

        private async Task SeedGrammarTestsAsync()
        {
            var existingTests = await _context.GrammarTests.ToListAsync();
            var hasValidTests = existingTests.Any(t => !string.IsNullOrEmpty(t.QuestionsText));

            if (hasValidTests)
            {
                _logger.LogInformation("Грамматические тесты с вопросами уже существуют");
                return;
            }

            var emptyTests = existingTests.Where(t => string.IsNullOrEmpty(t.QuestionsText)).ToList();
            if (emptyTests.Any())
            {
                _context.GrammarTests.RemoveRange(emptyTests);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Удалено {emptyTests.Count} пустых тестов");
            }

            var presentSimple = await _context.GrammarTopics
                .FirstOrDefaultAsync(t => t.Title == "Present Simple");

            if (presentSimple == null)
            {
                _logger.LogWarning("Тема Present Simple не найдена, тесты не добавлены");
                return;
            }

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
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            QuestionsText = @"I ___ to school every day. go | goes | going | went
She ___ English very well. speak | speaks | speaking | spoke
___ you like coffee? Do | Does | Is | Are
We ___ to the park on Sundays. go | goes | going | went
He ___ breakfast at 8 AM. have | has | having | had",
            AnswersText = @"go
speaks
Do
go
has"
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
            _logger.LogInformation($"Добавлено {tests.Count} грамматических тестов с вопросами");
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

        public async Task SeedAdminUserAsync()
        {
            var adminExists = await _context.Users.AnyAsync(u => u.Role == "Administrator");
            if (adminExists)
            {
                _logger.LogInformation("Администратор уже существует");
                return;
            }

            var adminPassword = BCrypt.Net.BCrypt.HashPassword("Admin123!");

            var admin = new User
            {
                UserName = "admin",
                Email = "admin@learnenglish.com",
                PasswordHash = adminPassword,
                Level = "C2",
                Role = "Administrator",
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(admin);
            await _context.SaveChangesAsync();

            await CreateDefaultCollectionsForUser(admin.Id);

            _logger.LogInformation("Администратор создан: Email=admin@learnenglish.com, Пароль=Admin123!");
        }

        private async Task UpdateGrammarTopicsContentAsync()
        {
            _logger.LogInformation("Обновляем контент грамматических уроков...");

            var presentSimple = await _context.GrammarTopics
                .FirstOrDefaultAsync(t => t.Title == "Present Simple");

            if (presentSimple != null && string.IsNullOrEmpty(presentSimple.TheoryContent))
            {
                presentSimple.TheoryContent = @"Present Simple (настоящее простое время)

Когда используется:
- Обычные, повторяющиеся действия
- Факты и общие истины
- Расписания и графики

Образование утвердительных предложений:
I/You/We/They + глагол (без окончания)
He/She/It + глагол + s/es

Образование отрицательных предложений:
I/You/We/They + do not (don't) + глагол
He/She/It + does not (doesn't) + глагол

Образование вопросительных предложений:
Do/Does + подлежащее + глагол?";

                presentSimple.ExamplesContent = @"I work every day.
She works in an office.
Water boils at 100 degrees.
The train leaves at 6 PM.";

                _context.GrammarTopics.Update(presentSimple);
            }

            var pastSimple = await _context.GrammarTopics
                .FirstOrDefaultAsync(t => t.Title == "Past Simple");

            if (pastSimple != null && string.IsNullOrEmpty(pastSimple.TheoryContent))
            {
                pastSimple.TheoryContent = @"Past Simple (прошедшее простое время)

Когда используется:
- Действия, которые произошли в прошлом
- Последовательные действия в прошлом

Правильные глаголы:
глагол + ed (work → worked)

Неправильные глаголы:
используется 2-я форма глагола (go → went)";

                pastSimple.ExamplesContent = @"I worked yesterday.
She went to London last year.
They watched a movie yesterday.";

                _context.GrammarTopics.Update(pastSimple);
            }

            var futureSimple = await _context.GrammarTopics
                .FirstOrDefaultAsync(t => t.Title == "Future Simple");

            if (futureSimple != null && string.IsNullOrEmpty(futureSimple.TheoryContent))
            {
                futureSimple.TheoryContent = @"Future Simple (будущее простое время)

Когда используется:
- Действия, которые произойдут в будущем
- Спонтанные решения
- Предсказания

Образование:
will + глагол (без частицы to)";

                futureSimple.ExamplesContent = @"I will call you tomorrow.
She will be here soon.
It will rain later.";

                _context.GrammarTopics.Update(futureSimple);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Контент грамматических уроков обновлён");
        }

        private async Task UpdateVocabLessonsContentAsync()
        {
            _logger.LogInformation("Обновляем контент вокабулярных уроков...");

            var dailyRoutine = await _context.VocabLessons
                .FirstOrDefaultAsync(l => l.Title == "My Daily Routine");

            if (dailyRoutine != null && string.IsNullOrEmpty(dailyRoutine.TheoryContent))
            {
                dailyRoutine.TheoryContent = @"Daily Routine (повседневные дела)

Повседневные дела - это действия, которые мы выполняем каждый день.

Основные глаголы:
- wake up - просыпаться
- get dressed - одеваться
- have breakfast - завтракать
- go to work/school - идти на работу/в школу
- have lunch - обедать
- come home - возвращаться домой
- have dinner - ужинать
- go to bed - ложиться спать";

                dailyRoutine.ExamplesContent = @"I wake up at 7 AM every day.
I have breakfast at 8 AM.
I go to work at 9 AM.
I have lunch at 1 PM.
I come home at 6 PM.
I have dinner at 7 PM.
I go to bed at 11 PM.";

                _context.VocabLessons.Update(dailyRoutine);
            }

            var myFamily = await _context.VocabLessons
                .FirstOrDefaultAsync(l => l.Title == "My Family");

            if (myFamily != null && string.IsNullOrEmpty(myFamily.TheoryContent))
            {
                myFamily.TheoryContent = @"Family (семья)

Семья - это самые близкие люди.

Члены семьи:
- mother/mom - мама
- father/dad - папа
- brother - брат
- sister - сестра
- grandmother - бабушка
- grandfather - дедушка
- aunt - тётя
- uncle - дядя
- cousin - двоюродный брат/сестра";

                myFamily.ExamplesContent = @"I have a big family.
My mother is a doctor.
My father works in an office.
I have two brothers and one sister.
My grandmother lives with us.";

                _context.VocabLessons.Update(myFamily);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Контент вокабулярных уроков обновлён");
        }


        private async Task CreateDefaultCollectionsForUser(long userId)
        {
            var collections = new[]
            {
        new Collection { UserId = userId, Name = "В процессе", IsDefault = true, CreatedAt = DateTime.UtcNow },
        new Collection { UserId = userId, Name = "Выученные", IsDefault = true, CreatedAt = DateTime.UtcNow },
            };

            await _context.Collections.AddRangeAsync(collections);
            await _context.SaveChangesAsync();
        }
    }
}