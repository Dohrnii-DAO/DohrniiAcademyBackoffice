using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Domain.Entities
{
    [Table("User")]
    public partial class User
    {
        public User()
        {
            ChapterActivities = new HashSet<ChapterActivity>();
            EarningActivities = new HashSet<EarningActivity>();
            FriendRequests = new HashSet<FriendRequest>();
            LessonActivities = new HashSet<LessonActivity>();
            LessonClassActivities = new HashSet<LessonClassActivity>();
            QuestionAttempts = new HashSet<QuestionAttempt>();
            QuizAttempts = new HashSet<QuizAttempt>();
            QuizUnlockActivities = new HashSet<QuizUnlockActivity>();
            WithdrawActivities = new HashSet<WithdrawActivity>();
        }

        [Key]
        public int Id { get; set; }
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
        [StringLength(15)]
        [Unicode(false)]
        public string? Phone { get; set; }
        [Unicode(false)]
        public string? WalletAddress { get; set; }
        [Unicode(false)]
        public string? ProfileImage { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateAdded { get; set; }
        [Column("TotalXP")]
        public int TotalXp { get; set; }
        public int TotalJelly { get; set; }
        [Column("TotalDHN", TypeName = "decimal(18, 4)")]
        public decimal TotalDhn { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string UserRole { get; set; } = null!;

        [InverseProperty("User")]
        public virtual ICollection<ChapterActivity> ChapterActivities { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<EarningActivity> EarningActivities { get; set; }
        [InverseProperty("Reciever")]
        public virtual ICollection<FriendRequest> FriendRequests { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<LessonActivity> LessonActivities { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<LessonClassActivity> LessonClassActivities { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<QuestionAttempt> QuestionAttempts { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<QuizAttempt> QuizAttempts { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<QuizUnlockActivity> QuizUnlockActivities { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<WithdrawActivity> WithdrawActivities { get; set; }
    }
}
