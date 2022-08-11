using System.ComponentModel.DataAnnotations;

namespace DohrniiBackoffice.Models
{
    public class mAnswer
    {
        public int ClassQuestionAnswerId { get; set; }
        public int ClassQuestionId { get; set; }
        [Required]
        public string AnswerOption { get; set; } = null!;
        public bool IsAnswer { get; set; }
    }
}
