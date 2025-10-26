using FPP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Infrastructure.Persistence
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ActivityType> ActivityTypes { get; set; }

        public virtual DbSet<EventParticipant> EventParticipants { get; set; }

        public virtual DbSet<Lab> Labs { get; set; }

        public virtual DbSet<LabEvent> LabEvents { get; set; }

        public virtual DbSet<LabZone> LabZones { get; set; }

        public virtual DbSet<Notification> Notifications { get; set; }

        public virtual DbSet<Report> Reports { get; set; }

        public virtual DbSet<SecurityLog> SecurityLogs { get; set; }

        public virtual DbSet<User> Users { get; set; }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//            => optionsBuilder.UseSqlServer("Server=LAPTOP-8UGAAJKM\\MSSQLSERVER01;Database=LabManagement;User Id=sa;Password=12345;TrustServerCertificate=True;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityType>(entity =>
            {
                entity.HasKey(e => e.ActivityTypeId).HasName("PK__activity__D2470C87629CC28F");

                entity.ToTable("activity_types");

                entity.Property(e => e.ActivityTypeId).HasColumnName("activity_type_id");
                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .HasColumnName("description");
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<EventParticipant>(entity =>
            {
                entity.HasKey(e => new { e.EventId, e.UserId }).HasName("PK__event_pa__C8EB1457EABB0E83");

                entity.ToTable("event_participants");

                entity.Property(e => e.EventId).HasColumnName("event_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Role)
                    .HasColumnType("decimal(2, 0)")
                    .HasColumnName("role");

                entity.HasOne(d => d.Event).WithMany(p => p.EventParticipants)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__event_par__event__49C3F6B7");

                entity.HasOne(d => d.User).WithMany(p => p.EventParticipants)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__event_par__user___4AB81AF0");
            });

            modelBuilder.Entity<Lab>(entity =>
            {
                entity.HasKey(e => e.LabId).HasName("PK__labs__66DE64DBFF9798EE");

                entity.ToTable("labs");

                entity.Property(e => e.LabId).HasColumnName("lab_id");
                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .HasColumnName("description");
                entity.Property(e => e.Location)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("location");
                entity.Property(e => e.ManagerId).HasColumnName("manager_id");
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.HasOne(d => d.Manager).WithMany(p => p.Labs)
                    .HasForeignKey(d => d.ManagerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__labs__manager_id__3B75D760");
            });

            modelBuilder.Entity<LabEvent>(entity =>
            {
                entity.HasKey(e => e.EventId).HasName("PK__lab_even__2370F7271B17C676");

                entity.ToTable("lab_events");

                entity.Property(e => e.EventId).HasColumnName("event_id");
                entity.Property(e => e.ActivityTypeId).HasColumnName("activity_type_id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .HasColumnName("description");
                entity.Property(e => e.EndTime)
                    .HasColumnType("datetime")
                    .HasColumnName("end_time");
                entity.Property(e => e.LabId).HasColumnName("lab_id");
                entity.Property(e => e.OrganizerId).HasColumnName("organizer_id");
                entity.Property(e => e.StartTime)
                    .HasColumnType("datetime")
                    .HasColumnName("start_time");
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("status");
                entity.Property(e => e.Title)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("title");
                entity.Property(e => e.ZoneId).HasColumnName("zone_id");

                entity.HasOne(d => d.ActivityType).WithMany(p => p.LabEvents)
                    .HasForeignKey(d => d.ActivityTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__lab_event__activ__45F365D3");

                entity.HasOne(d => d.Lab).WithMany(p => p.LabEvents)
                    .HasForeignKey(d => d.LabId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__lab_event__lab_i__440B1D61");

                entity.HasOne(d => d.Organizer).WithMany(p => p.LabEvents)
                    .HasForeignKey(d => d.OrganizerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__lab_event__organ__46E78A0C");

                entity.HasOne(d => d.Zone).WithMany(p => p.LabEvents)
                    .HasForeignKey(d => d.ZoneId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__lab_event__zone___44FF419A");
            });

            modelBuilder.Entity<LabZone>(entity =>
            {
                entity.HasKey(e => e.ZoneId).HasName("PK__lab_zone__80B401DFEA5810CC");

                entity.ToTable("lab_zones");

                entity.Property(e => e.ZoneId).HasColumnName("zone_id");
                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .HasColumnName("description");
                entity.Property(e => e.LabId).HasColumnName("lab_id");
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.HasOne(d => d.Lab).WithMany(p => p.LabZones)
                    .HasForeignKey(d => d.LabId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__lab_zones__lab_i__3E52440B");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.NotificationId).HasName("PK__notifica__E059842F693CAC11");

                entity.ToTable("notifications");

                entity.Property(e => e.NotificationId).HasColumnName("notification_id");
                entity.Property(e => e.EventId).HasColumnName("event_id");
                entity.Property(e => e.IsRead).HasColumnName("is_read");
                entity.Property(e => e.Message)
                    .HasColumnType("text")
                    .HasColumnName("message");
                entity.Property(e => e.RecipientId).HasColumnName("recipient_id");
                entity.Property(e => e.SentAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("sent_at");

                entity.HasOne(d => d.Event).WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__notificat__event__5535A963");

                entity.HasOne(d => d.Recipient).WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.RecipientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__notificat__recip__5441852A");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.ReportId).HasName("PK__reports__779B7C581AA75571");

                entity.ToTable("reports");

                entity.Property(e => e.ReportId).HasColumnName("report_id");
                entity.Property(e => e.Content)
                    .HasColumnType("text")
                    .HasColumnName("content");
                entity.Property(e => e.GeneratedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("generated_at");
                entity.Property(e => e.GeneratedBy).HasColumnName("generated_by");
                entity.Property(e => e.LabId).HasColumnName("lab_id");
                entity.Property(e => e.ReportType)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("report_type");
                entity.Property(e => e.ZoneId).HasColumnName("zone_id");

                entity.HasOne(d => d.GeneratedByNavigation).WithMany(p => p.Reports)
                    .HasForeignKey(d => d.GeneratedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__reports__generat__59063A47");

                entity.HasOne(d => d.Lab).WithMany(p => p.Reports)
                    .HasForeignKey(d => d.LabId)
                    .HasConstraintName("FK__reports__lab_id__59FA5E80");

                entity.HasOne(d => d.Zone).WithMany(p => p.Reports)
                    .HasForeignKey(d => d.ZoneId)
                    .HasConstraintName("FK__reports__zone_id__5AEE82B9");
            });

            modelBuilder.Entity<SecurityLog>(entity =>
            {
                entity.HasKey(e => e.LogId).HasName("PK__security__9E2397E03FC0E053");

                entity.ToTable("security_logs");

                entity.Property(e => e.LogId).HasColumnName("log_id");
                entity.Property(e => e.Action)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("action");
                entity.Property(e => e.EventId).HasColumnName("event_id");
                entity.Property(e => e.Notes)
                    .HasColumnType("text")
                    .HasColumnName("notes");
                entity.Property(e => e.PhotoUrl)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("photo_url");
                entity.Property(e => e.SecurityId).HasColumnName("security_id");
                entity.Property(e => e.Timestamp)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp");

                entity.HasOne(d => d.Event).WithMany(p => p.SecurityLogs)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__security___event__4E88ABD4");

                entity.HasOne(d => d.Security).WithMany(p => p.SecurityLogs)
                    .HasForeignKey(d => d.SecurityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__security___secur__4F7CD00D");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370F42B9A65E");

                entity.ToTable("users");

                entity.HasIndex(e => e.Email, "UQ__users__AB6E616485912C3D").IsUnique();

                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("email");
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("name");
                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasColumnName("password_hash");
                entity.Property(e => e.Role)
                    .HasColumnType("decimal(2, 0)")
                    .HasColumnName("role");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
 