using System.ComponentModel.DataAnnotations;

namespace DohrniiBackoffice.DTO.Request
{
    public class UnlockChapterDTO
    {
        [Required]
        public int ChapterId { get; set; }
    }
}
