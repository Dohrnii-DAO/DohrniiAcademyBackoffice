using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Domain.Entities
{
    [Table("LessonActivity")]
    public partial class LessonActivity
    {
        [Key]
        public int Id { get; set; }
        public int LessonId { get; set; }
        public int ChapterId { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public bool IsCompleted { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateStarted { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateCompleted { get; set; }

        [ForeignKey("CategoryId")]
        [InverseProperty("LessonActivities")]
        public virtual Category Category { get; set; } = null!;
        [ForeignKey("ChapterId")]
        [InverseProperty("LessonActivities")]
        public virtual Chapter Chapter { get; set; } = null!;
        [ForeignKey("LessonId")]
        [InverseProperty("LessonActivities")]
        public virtual Lesson Lesson { get; set; } = null!;
        [ForeignKey("UserId")]
        [InverseProperty("LessonActivities")]
        public virtual User User { get; set; } = null!;
    }
}
