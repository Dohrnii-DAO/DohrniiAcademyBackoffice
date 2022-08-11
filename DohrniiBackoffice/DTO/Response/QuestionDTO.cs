namespace DohrniiBackoffice.DTO.Response
{
    public class QuestionDTO
    {
        public int Id { get; set; }
        public int LessonClassId { get; set; }
        public string Question { get; set; }
        public string QuestionType { get; set; }
        public string Tags { get; set; }
    }
}
