using System;
using System.Collections.Generic;
using DAO.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAO;

public partial class BadmintonCourtContext : DbContext
{
    public BadmintonCourtContext()
    {
    }

    public BadmintonCourtContext(DbContextOptions<BadmintonCourtContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Balance> Balances { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Court> Courts { get; set; }

    public virtual DbSet<CourtActiveSlot> CourtActiveSlots { get; set; }

    public virtual DbSet<CourtBranch> CourtBranches { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Slot> Slots { get; set; }

    public virtual DbSet<StaffInBranch> StaffInBranches { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserDetail> UserDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost,1433;Database=BadmintonCourt;UID=sa;PWD=12345;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Balance>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Balance__CB9A1CDF620FE300");

            entity.ToTable("Balance");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("userID");
            entity.Property(e => e.Balance1).HasColumnName("balance");

            entity.HasOne(d => d.User).WithOne(p => p.Balance)
                .HasForeignKey<Balance>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Balance__userID__164452B1");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__C6D03BED0F68152C");

            entity.ToTable("Booking");

            entity.Property(e => e.BookingId)
                .ValueGeneratedNever()
                .HasColumnName("bookingID");
            entity.Property(e => e.BookingStatus).HasColumnName("bookingStatus");
            entity.Property(e => e.BookingType).HasColumnName("bookingType");
            entity.Property(e => e.TotalPrice).HasColumnName("totalPrice");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Booking__userID__267ABA7A");

            entity.HasMany(d => d.CourtActiveSlots).WithMany(p => p.Bookings)
                .UsingEntity<Dictionary<string, object>>(
                    "BookedSlot",
                    r => r.HasOne<CourtActiveSlot>().WithMany()
                        .HasForeignKey("CourtId", "SlotId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__BookedSlots__2E1BDC42"),
                    l => l.HasOne<Booking>().WithMany()
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__BookedSlo__booki__2D27B809"),
                    j =>
                    {
                        j.HasKey("BookingId", "CourtId", "SlotId").HasName("PK__BookedSl__A0AA92E2438BF1CD");
                        j.ToTable("BookedSlots");
                        j.IndexerProperty<int>("BookingId").HasColumnName("bookingID");
                        j.IndexerProperty<int>("CourtId").HasColumnName("courtID");
                        j.IndexerProperty<int>("SlotId").HasColumnName("slotID");
                    });
        });

        modelBuilder.Entity<Court>(entity =>
        {
            entity.HasKey(e => e.CourtId).HasName("PK__Court__4E6E3688AB984A7C");

            entity.ToTable("Court");

            entity.Property(e => e.CourtId)
                .ValueGeneratedNever()
                .HasColumnName("courtID");
            entity.Property(e => e.BranchId).HasColumnName("branchID");
            entity.Property(e => e.CourtImg)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("courtImg");
            entity.Property(e => e.CourtNumber).HasColumnName("courtNumber");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Price).HasColumnName("price");

            entity.HasOne(d => d.Branch).WithMany(p => p.Courts)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK__Court__branchID__1DE57479");
        });

        modelBuilder.Entity<CourtActiveSlot>(entity =>
        {
            entity.HasKey(e => new { e.CourtId, e.SlotId }).HasName("PK__CourtAct__67AA90F7322D2452");

            entity.Property(e => e.CourtId).HasColumnName("courtID");
            entity.Property(e => e.SlotId).HasColumnName("slotID");

            entity.HasOne(d => d.Court).WithMany(p => p.CourtActiveSlots)
                .HasForeignKey(d => d.CourtId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourtActi__court__29572725");

            entity.HasOne(d => d.Slot).WithMany(p => p.CourtActiveSlots)
                .HasForeignKey(d => d.SlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourtActi__slotI__2A4B4B5E");
        });

        modelBuilder.Entity<CourtBranch>(entity =>
        {
            entity.HasKey(e => e.BranchId).HasName("PK__CourtBra__751EBD3FDA269555");

            entity.ToTable("CourtBranch");

            entity.Property(e => e.BranchId)
                .ValueGeneratedNever()
                .HasColumnName("branchID");
            entity.Property(e => e.BranchImg)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("branchImg");
            entity.Property(e => e.BranchName)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("branchName");
            entity.Property(e => e.BranchPhone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("branchPhone");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("location");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__2613FD248129A660");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId)
                .ValueGeneratedNever()
                .HasColumnName("feedbackId");
            entity.Property(e => e.BranchId).HasColumnName("branchID");
            entity.Property(e => e.Content)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("content");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.Branch).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK__Feedback__branch__239E4DCF");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Feedback__userID__22AA2996");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__A0D9EFA6A286D632");

            entity.ToTable("Payment");

            entity.HasIndex(e => e.BookingId, "UQ__Payment__C6D03BECEAD591D2").IsUnique();

            entity.Property(e => e.PaymentId)
                .ValueGeneratedNever()
                .HasColumnName("paymentID");
            entity.Property(e => e.BookingId).HasColumnName("bookingID");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.Booking).WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.BookingId)
                .HasConstraintName("FK__Payment__booking__32E0915F");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Payment__userID__31EC6D26");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__CD98460A9442221C");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("roleID");
            entity.Property(e => e.Role1)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("role");
        });

        modelBuilder.Entity<Slot>(entity =>
        {
            entity.HasKey(e => e.SlotId).HasName("PK__Slot__9C4A67F39F401DF1");

            entity.ToTable("Slot");

            entity.Property(e => e.SlotId)
                .ValueGeneratedNever()
                .HasColumnName("slotID");
            entity.Property(e => e.EndTime).HasColumnName("endTime");
            entity.Property(e => e.StartTime).HasColumnName("startTime");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<StaffInBranch>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__StaffInB__CB9A1CDFF23CD875");

            entity.ToTable("StaffInBranch");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("userID");
            entity.Property(e => e.BranchId).HasColumnName("branchID");

            entity.HasOne(d => d.Branch).WithMany(p => p.StaffInBranches)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK__StaffInBr__branc__36B12243");

            entity.HasOne(d => d.User).WithOne(p => p.StaffInBranch)
                .HasForeignKey<StaffInBranch>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StaffInBr__userI__35BCFE0A");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__CB9A1CDFC5D66693");

            entity.ToTable("User");

            entity.HasIndex(e => e.UserName, "UQ__User__66DCF95CC507E621").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("userID");
            entity.Property(e => e.Password)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("roleID");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("userName");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__User__roleID__1367E606");
        });

        modelBuilder.Entity<UserDetail>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserDeta__CB9A1CDF69CAC423");

            entity.ToTable("UserDetail");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("userID");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("firstname");
            entity.Property(e => e.LastName)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("lastName");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone");

            entity.HasOne(d => d.User).WithOne(p => p.UserDetail)
                .HasForeignKey<UserDetail>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserDetai__userI__1920BF5C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
