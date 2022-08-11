using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Domain.Entities
{
    [Table("LessonClass")]
    public partial class LessonClass
    {
        public LessonClass()
        {
            ClassQuestions = new HashSet<ClassQuestion>();
            LessonClassActivities = new HashSet<LessonClassActivity>();
        }

        [Key]
        public int Id { get; set; }
        public int LessonId { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string Name { get; set; } = null!;
        [StringLength(1000)]
        [Unicode(false)]
        public string Title { get; set; } = null!;
        public int QuestionLimit { get; set; }
        public int XpPerQuestion { get; set; }
        [Unicode(false)]
        public string? CoverImage { get; set; }
        [Unicode(false)]
        public string? PdfContent { get; set; }
        [Unicode(false)]
        public string? HtmlContent { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateAdded { get; set; }
        public int Sequence { get; set; }

        [ForeignKey("LessonId")]
        [InverseProperty("LessonClasses")]
        public virtual Lesson Lesson { get; set; } = null!;
        [InverseProperty("LessonClass")]
        public virtual ICollection<ClassQuestion> ClassQuestions { get; set; }
        [InverseProperty("LessonClass")]
        public virtual ICollection<LessonClassActivity> LessonClassActivities { get; set; }
    }
}
