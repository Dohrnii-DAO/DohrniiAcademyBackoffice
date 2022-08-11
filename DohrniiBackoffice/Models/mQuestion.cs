using System.ComponentModel.DataAnnotations;

namespace DohrniiBackoffice.Models
{
    public class mQuestion
    {

        public int QuestionId { get; set; }
        public int LessonClassId { get; set; }

        [Required]
        public string Question { get; set; } = null!;

        [Required]
        public string QuestionType { get; set; } = null!;
        public string Tags { get; set; }
        public DateTime? DateAdded { get; set; }
    }
}
