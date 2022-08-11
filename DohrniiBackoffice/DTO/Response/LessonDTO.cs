namespace DohrniiBackoffice.DTO.Response
{
    public class LessonDTO
    {
        public int Id { get; set; }
        public int ChapterId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int Sequence { get; set; }
        public int TotalClass { get; set; }
        public int CompletedClass { get; set; }
        public bool IsStarted { get; set; }
        public bool IsCompleted { get; set; }
        public int TotalXPEarned { get; set; }
        public int TotalJellyEarned { get; set; }
        public int TotalXP { get; set; }

        public List<ClassDTO> Classes { get; set; }
    }
}
