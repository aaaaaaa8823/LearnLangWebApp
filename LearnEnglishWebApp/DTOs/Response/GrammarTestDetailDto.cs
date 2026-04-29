namespace LearnEnglishWebApp.DTOs.Response
{
    public class GrammarTestDetailDto
    {
        public long Id { get; set; }
        public long GrammarTopicId {  get; set; }
        public string TopicTitle {  get; set; }
        public string Level {  get; set; }
        public int QuestionCount { get; set; }
        public int OrderIndex { get; set; }
        public int PassingScore {  get; set; }
        public int TimeLimitMinutes { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
        public int? BestScore { get; set; }
        public double? BestPercentage { get; set; }
        public int AttemptCount { get; set; }

    }

    public class SubmitTestResultDto
    {
        public long TestId {  get; set; }
        public int Score { get; set; }
        public int MaxScore {  get; set; }
        public double Percentage { get; set; }
        public int TimeSpentSeconds { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public List<TestAnswerDetailDto> Answers { get; set; }
    }

    public class TestAnswerDetailDto
    {
        public int QuestionId { get; set; }
        public string UserAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public int PointsEarned { get; set; }
        public int MaxPoints { get; set; }
    }
}

