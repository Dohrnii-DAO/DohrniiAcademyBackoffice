using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Domain.Entities
{
    [Table("QuestionAttempt")]
    public partial class QuestionAttempt
    {
        [Key]
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int SelectedAnswerId { get; set; }
        public int UserId { get; set; }
        public bool IsCorrect { get; set; }
        [Column("XPCollected")]
        public int Xpcollected { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateAttempt { get; set; }

        [ForeignKey("QuestionId")]
        [InverseProperty("QuestionAttempts")]
        public virtual ClassQuestion Question { get; set; } = null!;
        [ForeignKey("SelectedAnswerId")]
        [InverseProperty("QuestionAttempts")]
        public virtual ClassQuestionAnswer SelectedAnswer { get; set; } = null!;
        [ForeignKey("UserId")]
        [InverseProperty("QuestionAttempts")]
        public virtual User User { get; set; } = null!;
    }
}
