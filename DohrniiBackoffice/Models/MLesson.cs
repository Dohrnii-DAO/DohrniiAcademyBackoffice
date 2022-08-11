using System.ComponentModel.DataAnnotations;

namespace DohrniiBackoffice.Models
{
    public class MLesson
    {
        public int LessonId { get; set; }
        [Required]
        public int ChapterId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public int Sequence { get; set; }
    }
}
