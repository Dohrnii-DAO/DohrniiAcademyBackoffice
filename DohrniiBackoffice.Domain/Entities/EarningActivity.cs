using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Domain.Entities
{
    [Table("EarningActivity")]
    public partial class EarningActivity
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public int ChapterId { get; set; }
        public int LessonId { get; set; }
        public int ClassId { get; set; }
        public int Jelly { get; set; }
        [Column("XP")]
        public int Xp { get; set; }
        [Column("DHN", TypeName = "decimal(18, 4)")]
        public decimal Dhn { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateAdded { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("EarningActivities")]
        public virtual User User { get; set; } = null!;
    }
}
