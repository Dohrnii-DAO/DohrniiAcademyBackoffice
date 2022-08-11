using System.ComponentModel.DataAnnotations;

namespace DohrniiBackoffice.Models
{
    public class MChapter
    {
        public int ChapterId { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Title { get; set; }
        public int RequiredJelly { get; set; }
        public int QuestionLimit { get; set; }
        public int TimeLimit { get; set; }
        public decimal RewardEighty { get; set; }
        public decimal RewardNinety { get; set; }
        public decimal RewardHundred { get; set; }
        public int Sequence { get; set; }
        public string? CoverImage { get; set; }
    }
}
