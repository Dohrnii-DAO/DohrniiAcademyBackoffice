namespace DohrniiBackoffice.DTO.Response
{
    public class LessonInprogress
    {
        public int CategoryId { get; set; }
        public int ChapterId { get; set; }

        public int LessonId { get; set; }
        public string LessonName { get; set; }
        public bool IsNotStarted { get; set; }

    }
}
