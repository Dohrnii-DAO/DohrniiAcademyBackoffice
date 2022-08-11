using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Domain.Entities
{
    [Table("Chapter")]
    public partial class Chapter
    {
        public Chapter()
        {
            ChapterActivities = new HashSet<ChapterActivity>();
            LessonActivities = new HashSet<LessonActivity>();
            LessonClassActivities = new HashSet<LessonClassActivity>();
            Lessons = new HashSet<Lesson>();
            QuizUnlockActivities = new HashSet<QuizUnlockActivity>();
        }

        [Key]
        public int Id { get; set; }
        public int CategoryId { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string Name { get; set; } = null!;
        [StringLength(1000)]
        [Unicode(false)]
        public string Title { get; set; } = null!;
        public int RequiredJelly { get; set; }
        public int QuestionLimit { get; set; }
        public int TimeLimit { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal RewardEighty { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal RewardNinety { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal RewardHundred { get; set; }
        [Unicode(false)]
        public string? CoverImage { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateAdded { get; set; }
        public int Sequence { get; set; }

        [ForeignKey("CategoryId")]
        [InverseProperty("Chapters")]
        public virtual Category Category { get; set; } = null!;
        [InverseProperty("Chapter")]
        public virtual ICollection<ChapterActivity> ChapterActivities { get; set; }
        [InverseProperty("Chapter")]
        public virtual ICollection<LessonActivity> LessonActivities { get; set; }
        [InverseProperty("Chapter")]
        public virtual ICollection<LessonClassActivity> LessonClassActivities { get; set; }
        [InverseProperty("Chapter")]
        public virtual ICollection<Lesson> Lessons { get; set; }
        [InverseProperty("Chapter")]
        public virtual ICollection<QuizUnlockActivity> QuizUnlockActivities { get; set; }
    }
}
