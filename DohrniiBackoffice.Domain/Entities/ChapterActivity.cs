using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Domain.Entities
{
    [Table("ChapterActivity")]
    public partial class ChapterActivity
    {
        [Key]
        public int Id { get; set; }
        public int ChapterId { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public bool IsCompleted { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateStarted { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateCompleted { get; set; }

        [ForeignKey("CategoryId")]
        [InverseProperty("ChapterActivities")]
        public virtual Category Category { get; set; } = null!;
        [ForeignKey("ChapterId")]
        [InverseProperty("ChapterActivities")]
        public virtual Chapter Chapter { get; set; } = null!;
        [ForeignKey("UserId")]
        [InverseProperty("ChapterActivities")]
        public virtual User User { get; set; } = null!;
    }
}
