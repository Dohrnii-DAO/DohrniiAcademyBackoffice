namespace DohrniiBackoffice.DTO.Response
{
    public class ClassDTO
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int QuestionLimit { get; set; }
        public int XpPerQuestion { get; set; }
        public string? HtmlContent { get; set; }
        public int Sequence { get; set; }
        public bool IsStarted { get; set; }
        public bool IsCompleted { get; set; }
        public int TotalXP { get; set; }

    }
}
