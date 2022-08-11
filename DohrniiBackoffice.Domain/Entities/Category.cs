using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Domain.Entities
{
    [Table("Category")]
    public partial class Category
    {
        public Category()
        {
            ChapterActivities = new HashSet<ChapterActivity>();
            Chapters = new HashSet<Chapter>();
            LessonActivities = new HashSet<LessonActivity>();
            LessonClassActivities = new HashSet<LessonClassActivity>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string Name { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime? DateAdded { get; set; }
        [StringLength(1000)]
        [Unicode(false)]
        public string? Description { get; set; }

        [InverseProperty("Category")]
        public virtual ICollection<ChapterActivity> ChapterActivities { get; set; }
        [InverseProperty("Category")]
        public virtual ICollection<Chapter> Chapters { get; set; }
        [InverseProperty("Category")]
        public virtual ICollection<LessonActivity> LessonActivities { get; set; }
        [InverseProperty("Category")]
        public virtual ICollection<LessonClassActivity> LessonClassActivities { get; set; }
    }
}
