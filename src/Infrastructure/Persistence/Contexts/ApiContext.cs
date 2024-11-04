using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API;

public partial class ApiContext : DbContext
{
    public ApiContext()
    {
    }

    public ApiContext(DbContextOptions<ApiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<History> Histories { get; set; }

    public virtual DbSet<Mailbox> Mailboxes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=ConnectionStrings:Api");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<History>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("history_pkey");

            entity.ToTable("history");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdMailbox).HasColumnName("id_mailbox");
            entity.Property(e => e.MailWeight).HasColumnName("mail_weight");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.Time)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time");

            entity.HasOne(d => d.IdMailboxNavigation).WithMany(p => p.Histories)
                .HasForeignKey(d => d.IdMailbox)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("history_id_mailbox_fkey");
        });

        modelBuilder.Entity<Mailbox>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mailbox_pkey");

            entity.ToTable("mailbox");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MailWeight).HasColumnName("mail_weight");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Fcmtoken)
                .HasMaxLength(255)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("fcmtoken");
            entity.Property(e => e.IdMailbox).HasColumnName("id_mailbox");
            entity.Property(e => e.Notification)
                .HasDefaultValue(true)
                .HasColumnName("notification");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");

            entity.HasOne(d => d.IdMailboxNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdMailbox)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_id_mailbox_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
