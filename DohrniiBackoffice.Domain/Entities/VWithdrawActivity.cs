using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Domain.Entities
{
    [Keyless]
    public partial class VWithdrawActivity
    {
        public int Id { get; set; }
        public int Jelly { get; set; }
        [Column("XP")]
        public int Xp { get; set; }
        [Column("DHN", TypeName = "decimal(18, 18)")]
        public decimal Dhn { get; set; }
        public int UserId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateAdded { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string UserName { get; set; } = null!;
        [StringLength(300)]
        [Unicode(false)]
        public string Email { get; set; } = null!;
        [StringLength(50)]
        [Unicode(false)]
        public string FirstName { get; set; } = null!;
        [StringLength(50)]
        [Unicode(false)]
        public string LastName { get; set; } = null!;
    }
}
