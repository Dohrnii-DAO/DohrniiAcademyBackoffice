using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Domain.Entities
{
    [Table("LessonClassActivity")]
    public partial class LessonClassActivity
    {
        [Key]
        public int Id { get; set; }
        public int LessonClassId { get; set; }
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
        [InverseProperty("LessonClassActivities")]
        public virtual Category Category { get; set; } = null!;
        [ForeignKey("ChapterId")]
        [InverseProperty("LessonClassActivities")]
        public virtual Chapter Chapter { get; set; } = null!;
        [ForeignKey("LessonId")]
        [InverseProperty("LessonClassActivities")]
        public virtual Lesson Lesson { get; set; } = null!;
        [ForeignKey("LessonClassId")]
        [InverseProperty("LessonClassActivities")]
        public virtual LessonClass LessonClass { get; set; } = null!;
        [ForeignKey("UserId")]
        [InverseProperty("LessonClassActivities")]
        public virtual User User { get; set; } = null!;
    }
}
