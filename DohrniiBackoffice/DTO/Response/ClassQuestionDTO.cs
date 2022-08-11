namespace DohrniiBackoffice.DTO.Response
{
    public class ClassQuestionDTO
    {
        public int Id { get; set; }
        public int LessonClassId { get; set; }
        public string Question { get; set; }
        public string QuestionType { get; set; }
        public string Tags { get; set; }
        public bool IsAttempted { get; set; }
        public List<ClassQuestionOptionDTO> Options { get; set; }
        public List<ClassQuestionAttemptDTO> Attempts { get; set; }
    }
}
    