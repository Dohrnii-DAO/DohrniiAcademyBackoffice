namespace DohrniiBackoffice.DTO.Response
{
    public class CategoryProgressDTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int TotalLesson { get; set; }
        public int CompletedLesson { get; set; }
        public int TotalClass { get; set; }
        public int CompletedClass { get; set; }
    }
}
