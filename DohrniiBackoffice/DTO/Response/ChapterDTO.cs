namespace DohrniiBackoffice.DTO.Response
{
    public class ChapterDTO
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Name { get; set; } = null!;
        public string Title { get; set; }
        public int RequiredJelly { get; set; }
        public int QuestionLimit { get; set; }
        public int TimeLimit { get; set; }
        public decimal RewardEighty { get; set; }
        public decimal RewardNinety { get; set; }
        public decimal RewardHundred { get; set; }
        public int Sequence { get; set; }
        public int TotalClass { get; set; }
        public int CompletedClass { get; set; }
        public bool IsQuizUnlocked { get; set; }
        public bool IsStarted { get; set; }
        public bool IsCompleted { get; set; }
        public List<LessonDTO> Lessons { get; set; }
    }
}
