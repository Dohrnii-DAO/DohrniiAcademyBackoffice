namespace DohrniiBackoffice.DTO.Request
{
    public class QuestionAttemptDTO
    {
        public int QuestionId { get; set; }
        public int SelectedAnswerId { get; set; }
        public bool IsCorrect { get; set; }
        public int ClassId { get; set; }
        public int LessonId { get; set; }
        public int Xpcollected { get; set; }
    }
}
