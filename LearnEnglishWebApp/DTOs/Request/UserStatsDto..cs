namespace LearnEnglishWebApp.DTOs.Request
{
    public class UserStatsDto
    {
        public int LearningWords { get; set; }  // Слов в процессе
        public int LearnedWords { get; set; }   // Выученных слов
        public int CompletedTests { get; set; } // Пройденных тестов
        public int CompletedLessons { get; set; } // Пройденных уроков
        public DateTime MemberSince { get; set; } // Дата регистрации
    }
}
