using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Domain.Entities
{
    [Table("ClassQuestionAnswer")]
    public partial class ClassQuestionAnswer
    {
        public ClassQuestionAnswer()
        {
            QuestionAttempts = new HashSet<QuestionAttempt>();
            QuizAttempts = new HashSet<QuizAttempt>();
        }

        [Key]
        public int Id { get; set; }
        public int ClassQuestionId { get; set; }
        [Unicode(false)]
        public string AnswerOption { get; set; } = null!;
        public bool IsAnswer { get; set; }

        [ForeignKey("ClassQuestionId")]
        [InverseProperty("ClassQuestionAnswers")]
        public virtual ClassQuestion ClassQuestion { get; set; } = null!;
        [InverseProperty("SelectedAnswer")]
        public virtual ICollection<QuestionAttempt> QuestionAttempts { get; set; }
        [InverseProperty("SelectedAnswer")]
        public virtual ICollection<QuizAttempt> QuizAttempts { get; set; }
    }
}
