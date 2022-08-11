using System;
using System.Collections.Generic;
using DohrniiBackoffice.Domain.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DohrniiBackoffice.Domain.Entities
{
    public partial class DohrniiBackOfficeContext : DbContext, IDbContext
    {
        public DohrniiBackOfficeContext()
        {
        }

        public DohrniiBackOfficeContext(DbContextOptions<DohrniiBackOfficeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppSetting> AppSettings { get; set; } = null!;
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; } = null!;
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; } = null!;
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; } = null!;
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; } = null!;
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; } = null!;
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Chapter> Chapters { get; set; } = null!;
        public virtual DbSet<ChapterActivity> ChapterActivities { get; set; } = null!;
        public virtual DbSet<ClassQuestion> ClassQuestions { get; set; } = null!;
        public virtual DbSet<ClassQuestionAnswer> ClassQuestionAnswers { get; set; } = null!;
        public virtual DbSet<EarningActivity> EarningActivities { get; set; } = null!;
        public virtual DbSet<EmailVerification> EmailVerifications { get; set; } = null!;
        public virtual DbSet<FriendRequest> FriendRequests { get; set; } = null!;
        public virtual DbSet<Lesson> Lessons { get; set; } = null!;
        public virtual DbSet<LessonActivity> LessonActivities { get; set; } = null!;
        public virtual DbSet<LessonClass> LessonClasses { get; set; } = null!;
        public virtual DbSet<LessonClassActivity> LessonClassActivities { get; set; } = null!;
        public virtual DbSet<QuestionAttempt> QuestionAttempts { get; set; } = null!;
        public virtual DbSet<QuizAttempt> QuizAttempts { get; set; } = null!;
        public virtual DbSet<QuizUnlockActivity> QuizUnlockActivities { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<VEarningActivity> VEarningActivities { get; set; } = null!;
        public virtual DbSet<VFriendRequest> VFriendRequests { get; set; } = null!;
        public virtual DbSet<VQuestion> VQuestions { get; set; } = null!;
        public virtual DbSet<VWithdrawActivity> VWithdrawActivities { get; set; } = null!;
        public virtual DbSet<WithdrawActivity> WithdrawActivities { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=DefaultConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "AspNetUserRole",
                        l => l.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                        r => r.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId");

                            j.ToTable("AspNetUserRoles");

                            j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                        });
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
            });

            modelBuilder.Entity<Chapter>(entity =>
            {
                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Chapters)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Chapter_Category");
            });

            modelBuilder.Entity<ChapterActivity>(entity =>
            {
                entity.HasOne(d => d.Category)
                    .WithMany(p => p.ChapterActivities)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChapterActivity_Category");

                entity.HasOne(d => d.Chapter)
                    .WithMany(p => p.ChapterActivities)
                    .HasForeignKey(d => d.ChapterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChapterActivity_Chapter");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ChapterActivities)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_ChapterActivity_User");
            });

            modelBuilder.Entity<ClassQuestion>(entity =>
            {
                entity.HasOne(d => d.LessonClass)
                    .WithMany(p => p.ClassQuestions)
                    .HasForeignKey(d => d.LessonClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ClassQuestion_LessonClass");
            });

            modelBuilder.Entity<ClassQuestionAnswer>(entity =>
            {
                entity.HasOne(d => d.ClassQuestion)
                    .WithMany(p => p.ClassQuestionAnswers)
                    .HasForeignKey(d => d.ClassQuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ClassQuestionAnswer_ClassQuestion");
            });

            modelBuilder.Entity<EarningActivity>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.EarningActivities)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EarningActivity_User");
            });

            modelBuilder.Entity<FriendRequest>(entity =>
            {
                entity.HasOne(d => d.Reciever)
                    .WithMany(p => p.FriendRequests)
                    .HasForeignKey(d => d.RecieverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FriendRequest_User");
            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.HasOne(d => d.Chapter)
                    .WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.ChapterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Lesson_Chapter");
            });

            modelBuilder.Entity<LessonActivity>(entity =>
            {
                entity.HasOne(d => d.Category)
                    .WithMany(p => p.LessonActivities)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LessonActivity_Category");

                entity.HasOne(d => d.Chapter)
                    .WithMany(p => p.LessonActivities)
                    .HasForeignKey(d => d.ChapterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LessonActivity_Chapter");

                entity.HasOne(d => d.Lesson)
                    .WithMany(p => p.LessonActivities)
                    .HasForeignKey(d => d.LessonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LessonActivity_Lesson");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.LessonActivities)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LessonActivity_User");
            });

            modelBuilder.Entity<LessonClass>(entity =>
            {
                entity.HasOne(d => d.Lesson)
                    .WithMany(p => p.LessonClasses)
                    .HasForeignKey(d => d.LessonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LessonClass_Lesson");
            });

            modelBuilder.Entity<LessonClassActivity>(entity =>
            {
                entity.HasOne(d => d.Category)
                    .WithMany(p => p.LessonClassActivities)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LessonClassActivity_Category");

                entity.HasOne(d => d.Chapter)
                    .WithMany(p => p.LessonClassActivities)
                    .HasForeignKey(d => d.ChapterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LessonClassActivity_Chapter");

                entity.HasOne(d => d.LessonClass)
                    .WithMany(p => p.LessonClassActivities)
                    .HasForeignKey(d => d.LessonClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LessonClassActivity_LessonClass");

                entity.HasOne(d => d.Lesson)
                    .WithMany(p => p.LessonClassActivities)
                    .HasForeignKey(d => d.LessonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LessonClassActivity_Lesson");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.LessonClassActivities)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LessonClassActivity_User");
            });

            modelBuilder.Entity<QuestionAttempt>(entity =>
            {
                entity.HasOne(d => d.Question)
                    .WithMany(p => p.QuestionAttempts)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionAttempt_ClassQuestion");

                entity.HasOne(d => d.SelectedAnswer)
                    .WithMany(p => p.QuestionAttempts)
                    .HasForeignKey(d => d.SelectedAnswerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionAttempt_ClassQuestionAnswer");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.QuestionAttempts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionAttempt_User");
            });

            modelBuilder.Entity<QuizAttempt>(entity =>
            {
                entity.HasOne(d => d.Question)
                    .WithMany(p => p.QuizAttempts)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuizAttempt_ClassQuestion");

                entity.HasOne(d => d.SelectedAnswer)
                    .WithMany(p => p.QuizAttempts)
                    .HasForeignKey(d => d.SelectedAnswerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuizAttempt_ClassQuestionAnswer");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.QuizAttempts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuizAttempt_User");
            });

            modelBuilder.Entity<QuizUnlockActivity>(entity =>
            {
                entity.HasOne(d => d.Chapter)
                    .WithMany(p => p.QuizUnlockActivities)
                    .HasForeignKey(d => d.ChapterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuizUnlockActivity_Chapter");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.QuizUnlockActivities)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuizUnlockActivity_User");
            });

            modelBuilder.Entity<VEarningActivity>(entity =>
            {
                entity.ToView("vEarningActivity");
            });

            modelBuilder.Entity<VFriendRequest>(entity =>
            {
                entity.ToView("vFriendRequest");
            });

            modelBuilder.Entity<VQuestion>(entity =>
            {
                entity.ToView("vQuestion");
            });

            modelBuilder.Entity<VWithdrawActivity>(entity =>
            {
                entity.ToView("vWithdrawActivity");
            });

            modelBuilder.Entity<WithdrawActivity>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.WithdrawActivities)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WithdrawActivity_User");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
        public override int SaveChanges()
        {
            throw new InvalidOperationException("User ID must be provided");
        }

        public async Task<int> SaveChanges(string userId, string Ip)
        {
            try
            {
                //TODO

                // Call the original SaveChanges(), which will save both the changes made and the audit records
                return await base.SaveChangesAsync();
            }
            catch (Exception x)
            {
                throw x;
            }
        }
    }
}
