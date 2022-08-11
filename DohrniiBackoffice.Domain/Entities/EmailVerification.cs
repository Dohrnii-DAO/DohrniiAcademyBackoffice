using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Domain.Entities
{
    [Table("EmailVerification")]
    public partial class EmailVerification
    {
        [Key]
        public int Id { get; set; }
        [StringLength(300)]
        [Unicode(false)]
        public string Email { get; set; } = null!;
        [StringLength(7)]
        [Unicode(false)]
        public string Code { get; set; } = null!;
        [StringLength(50)]
        [Unicode(false)]
        public string? Region { get; set; }
        public bool Verified { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Device { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }
    }
}
