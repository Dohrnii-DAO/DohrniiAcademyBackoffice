namespace DohrniiBackoffice.DTO.Request
{
    public class QuizAttemptDTO
    {
        public int QuestionId { get; set; }
        public int SelectedAnswerId { get; set; }
        public bool IsCorrect { get; set; }
        public int Xpcollected { get; set; }
        public int ChapterId { get; set; }
    }
}
