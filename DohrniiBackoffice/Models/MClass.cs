using System.ComponentModel.DataAnnotations;

namespace DohrniiBackoffice.Models
{
    public class MClass
    {
        public int ClassId { get; set; }
        public int LessonId { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
        public int QuestionLimit { get; set; }
        public int XpPerQuestion { get; set; }
        public string HtmlContent { get; set; } = null!;
        public int Sequence { get; set; }
    }
}
