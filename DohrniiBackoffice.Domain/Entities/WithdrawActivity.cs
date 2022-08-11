using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Domain.Entities
{
    [Table("WithdrawActivity")]
    public partial class WithdrawActivity
    {
        [Key]
        public int Id { get; set; }
        public int Jelly { get; set; }
        [Column("XP")]
        public int Xp { get; set; }
        [Column("DHN", TypeName = "decimal(18, 4)")]
        public decimal Dhn { get; set; }
        public int UserId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateAdded { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("WithdrawActivities")]
        public virtual User User { get; set; } = null!;
    }
}
