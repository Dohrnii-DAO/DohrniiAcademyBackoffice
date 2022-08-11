using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Domain.Entities
{
    [Keyless]
    public partial class VFriendRequest
    {
        public int Id { get; set; }
        public int RequesterId { get; set; }
        public int RecieverId { get; set; }
        public bool IsAccepted { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateSent { get; set; }
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
        [Unicode(false)]
        public string? ProfileImage { get; set; }
    }
}
