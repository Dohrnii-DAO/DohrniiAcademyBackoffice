using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Domain.Entities
{
    [Table("FriendRequest")]
    public partial class FriendRequest
    {
        [Key]
        public int Id { get; set; }
        public int RequesterId { get; set; }
        public int RecieverId { get; set; }
        public bool IsAccepted { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateSent { get; set; }

        [ForeignKey("RecieverId")]
        [InverseProperty("FriendRequests")]
        public virtual User Reciever { get; set; } = null!;
    }
}
