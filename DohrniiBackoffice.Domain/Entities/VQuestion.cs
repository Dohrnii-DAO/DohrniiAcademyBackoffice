using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Domain.Entities
{
    [Keyless]
    public partial class VQuestion
    {
        public int Id { get; set; }
        public int LessonClassId { get; set; }
        public int LessonId { get; set; }
        public int ChapterId { get; set; }
        [Unicode(false)]
        public string Question { get; set; } = null!;
        [StringLength(50)]
        [Unicode(false)]
        public string QuestionType { get; set; } = null!;
        [Unicode(false)]
        public string Tags { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime? DateAdded { get; set; }
    }
}
