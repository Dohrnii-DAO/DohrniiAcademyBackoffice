using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Domain.Entities
{
    [Table("Lesson")]
    public partial class Lesson
    {
        public Lesson()
        {
            LessonActivities = new HashSet<LessonActivity>();
            LessonClassActivities = new HashSet<LessonClassActivity>();
            LessonClasses = new HashSet<LessonClass>();
        }

        [Key]
        public int Id { get; set; }
        public int ChapterId { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string Name { get; set; } = null!;
        [StringLength(1000)]
        [Unicode(false)]
        public string Title { get; set; } = null!;
        public int Sequence { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateAdded { get; set; }

        [ForeignKey("ChapterId")]
        [InverseProperty("Lessons")]
        public virtual Chapter Chapter { get; set; } = null!;
        [InverseProperty("Lesson")]
        public virtual ICollection<LessonActivity> LessonActivities { get; set; }
        [InverseProperty("Lesson")]
        public virtual ICollection<LessonClassActivity> LessonClassActivities { get; set; }
        [InverseProperty("Lesson")]
        public virtual ICollection<LessonClass> LessonClasses { get; set; }
    }
}
