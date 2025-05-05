using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Timesheet.Core.Entites;

public partial class TimesheetContext : DbContext
{
    public TimesheetContext()
    {
    }

    public TimesheetContext(DbContextOptions<TimesheetContext> options)
        : base(options)
    {
    }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<AttendanceLogs> AttendanceLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\ProjectModels;Initial Catalog=Timesheet;Integrated Security=True;Connect Timeout=300;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC077C7A4DD8");

            entity.ToTable("RefreshToken");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.ExpireDate).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_RefreshToken_User");
        });

        modelBuilder.Entity<AttendanceLogs>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Timeshee__3214EC07889F1934");

            entity.HasIndex(e => new { e.UserId, e.Date }, "UQ__Timeshee__60FB4B9D2A7B3CB3").IsUnique();

            entity.Property(e => e.LoginTime).HasColumnType("datetime");
            entity.Property(e => e.LogoutTime).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Timesheets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Timesheet__UserI__3C69FB99");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07BE6FB6AD");

            entity.HasIndex(e => e.MobileNumber, "UQ__Users__250375B192EA8386").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534A8C96FF3").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MobileNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
