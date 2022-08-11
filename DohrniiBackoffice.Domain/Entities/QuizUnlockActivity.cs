using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Domain.Entities
{
    [Table("QuizUnlockActivity")]
    public partial class QuizUnlockActivity
    {
        [Key]
        public int Id { get; set; }
        public int ChapterId { get; set; }
        public int UserId { get; set; }
        public int Jelly { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateUnlocked { get; set; }

        [ForeignKey("ChapterId")]
        [InverseProperty("QuizUnlockActivities")]
        public virtual Chapter Chapter { get; set; } = null!;
        [ForeignKey("UserId")]
        [InverseProperty("QuizUnlockActivities")]
        public virtual User User { get; set; } = null!;
    }
}
