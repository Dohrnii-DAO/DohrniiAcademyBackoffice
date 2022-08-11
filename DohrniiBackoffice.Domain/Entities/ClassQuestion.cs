using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Domain.Entities
{
    [Table("ClassQuestion")]
    public partial class ClassQuestion
    {
        public ClassQuestion()
        {
            ClassQuestionAnswers = new HashSet<ClassQuestionAnswer>();
            QuestionAttempts = new HashSet<QuestionAttempt>();
            QuizAttempts = new HashSet<QuizAttempt>();
        }

        [Key]
        public int Id { get; set; }
        public int LessonClassId { get; set; }
        [Unicode(false)]
        public string Question { get; set; } = null!;
        [StringLength(50)]
        [Unicode(false)]
        public string QuestionType { get; set; } = null!;
        [Unicode(false)]
        public string Tags { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime? DateAdded { get; set; }

        [ForeignKey("LessonClassId")]
        [InverseProperty("ClassQuestions")]
        public virtual LessonClass LessonClass { get; set; } = null!;
        [InverseProperty("ClassQuestion")]
        public virtual ICollection<ClassQuestionAnswer> ClassQuestionAnswers { get; set; }
        [InverseProperty("Question")]
        public virtual ICollection<QuestionAttempt> QuestionAttempts { get; set; }
        [InverseProperty("Question")]
        public virtual ICollection<QuizAttempt> QuizAttempts { get; set; }
    }
}
