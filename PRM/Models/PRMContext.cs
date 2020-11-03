using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PRM.Models
{
    public partial class PRMContext : DbContext
    {
        public PRMContext()
        {
        }

        public PRMContext(DbContextOptions<PRMContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Like> Like { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Video> Video { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
               // optionsBuilder.UseSqlServer("Server=localhost;Database=PRM;uid=haudq;password=123;Trusted_Connection=True;");
                optionsBuilder.UseSqlServer("Data Source=scamdbservers.database.windows.net;Initial Catalog=PRM;Persist Security Info=True;User ID=ad1999;Password=Anhdung99");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>(entity =>
            {
/*                entity.HasNoKey();*/

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.VideoId).HasColumnName("VideoID");


                entity.HasKey(sc => new { sc.UserId, sc.VideoId });

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.VideoId).HasColumnName("VideoID");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Comment_User");

                entity.HasOne(d => d.Video)
                    .WithMany()
                    .HasForeignKey(d => d.VideoId)
                    .HasConstraintName("FK_Comment_Video");
            });

            modelBuilder.Entity<Like>(entity =>
            {
              /*  entity.HasNoKey();*/

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.VideoId).HasColumnName("VideoID");

                entity.HasKey(sc => new { sc.UserId, sc.VideoId });

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.VideoId).HasColumnName("VideoID");


                // cấu hình key của api dưới db theo mô hình một nhiều
                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Like_User");

                entity.HasOne(d => d.Video)
                    .WithMany()
                    .HasForeignKey(d => d.VideoId)
                    .HasConstraintName("FK_Like_Video");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Birthday)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Fullname).HasMaxLength(100);

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Video>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Title)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.UrlShare).HasMaxLength(50);

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
