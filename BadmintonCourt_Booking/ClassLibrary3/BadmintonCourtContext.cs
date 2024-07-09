using System;
using System.Collections.Generic;
using ClassLibrary3.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary3;

public partial class BadmintonCourtContext : DbContext
{
    public BadmintonCourtContext()
    {
    }

    public BadmintonCourtContext(DbContextOptions<BadmintonCourtContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BookedSlot> BookedSlots { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Court> Courts { get; set; }

    public virtual DbSet<CourtBranch> CourtBranches { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserDetail> UserDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=(local);Database= BadmintonCourt;UID=sa;PWD=12345;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookedSlot>(entity =>
        {
            entity.HasKey(e => e.SlotId).HasName("PK__BookedSl__9C4A67F340752D9C");

            entity.ToTable("BookedSlot");

            entity.Property(e => e.SlotId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("slotID");
            entity.Property(e => e.BookingId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("bookingID");
            entity.Property(e => e.CourtId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("courtID");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("endTime");
            entity.Property(e => e.IsDelete).HasColumnName("isDelete");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("startTime");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookedSlots)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKBookedSlot690847");

            entity.HasOne(d => d.Court).WithMany(p => p.BookedSlots)
                .HasForeignKey(d => d.CourtId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKBookedSlot778580");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__C6D03BED41FE9562");

            entity.ToTable("Booking");

            entity.Property(e => e.BookingId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("bookingID");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.BookingDate)
                .HasColumnType("datetime")
                .HasColumnName("bookingDate");
            entity.Property(e => e.BookingType).HasColumnName("bookingType");
            entity.Property(e => e.ChangeLog).HasColumnName("changeLog");
            entity.Property(e => e.IsDelete).HasColumnName("isDelete");
            entity.Property(e => e.UserId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("userID");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKBooking923627");
        });

        modelBuilder.Entity<Court>(entity =>
        {
            entity.HasKey(e => e.CourtId).HasName("PK__Court__4E6E368830B36B74");

            entity.ToTable("Court");

            entity.Property(e => e.CourtId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("courtID");
            entity.Property(e => e.BranchId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("branchID");
            entity.Property(e => e.CourtImg)
                .HasMaxLength(3000)
                .IsUnicode(false)
                .HasColumnName("courtImg");
            entity.Property(e => e.CourtName)
                .HasMaxLength(30)
                .HasColumnName("courtName");
            entity.Property(e => e.CourtStatus).HasColumnName("courtStatus");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Price).HasColumnName("price");

            entity.HasOne(d => d.Branch).WithMany(p => p.Courts)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKCourt788847");
        });

        modelBuilder.Entity<CourtBranch>(entity =>
        {
            entity.HasKey(e => e.BranchId).HasName("PK__CourtBra__751EBD3FE88897B0");

            entity.ToTable("CourtBranch");

            entity.Property(e => e.BranchId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("branchID");
            entity.Property(e => e.BranchImg)
                .HasMaxLength(3000)
                .IsUnicode(false)
                .HasColumnName("branchImg");
            entity.Property(e => e.BranchName)
                .HasMaxLength(50)
                .HasColumnName("branchName");
            entity.Property(e => e.BranchPhone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("branchPhone");
            entity.Property(e => e.BranchStatus).HasColumnName("branchStatus");
            entity.Property(e => e.Location)
                .HasMaxLength(50)
                .HasColumnName("location");
            entity.Property(e => e.MapUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("mapUrl");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__D2130A06EBDFAFA2");

            entity.ToTable("Discount");

            entity.Property(e => e.DiscountId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("discountID");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.IsDelete).HasColumnName("isDelete");
            entity.Property(e => e.Proportion).HasColumnName("proportion");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__2613FDC4C0588C5F");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("feedbackID");
            entity.Property(e => e.BranchId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("branchID");
            entity.Property(e => e.Content)
                .HasMaxLength(500)
                .HasColumnName("content");
            entity.Property(e => e.IsDelete).HasColumnName("isDelete");
            entity.Property(e => e.Period)
                .HasColumnType("datetime")
                .HasColumnName("period");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UserId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("userID");

            entity.HasOne(d => d.Branch).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKFeedback632553");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FKFeedback274984");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__A0D9EFA66493AEE3");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("paymentID");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.BookingId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("bookingID");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Method).HasColumnName("method");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("transactionId");
            entity.Property(e => e.UserId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("userID");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FKPayment887923");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPayment444730");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__CD98460AE4E0E051");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("roleID");
            entity.Property(e => e.Role1)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("role");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__CB9A1CDFE7EF05FE");

            entity.ToTable("User");

            entity.Property(e => e.UserId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("userID");
            entity.Property(e => e.AccessFail).HasColumnName("accessFail");
            entity.Property(e => e.ActionPeriod)
                .HasColumnType("datetime")
                .HasColumnName("actionPeriod");
            entity.Property(e => e.ActiveStatus).HasColumnName("activeStatus");
            entity.Property(e => e.Balance).HasColumnName("balance");
            entity.Property(e => e.BranchId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("branchID");
            entity.Property(e => e.LastFail)
                .HasColumnType("datetime")
                .HasColumnName("lastFail");
            entity.Property(e => e.Password)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.RoleId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("roleID");
            entity.Property(e => e.Token)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("token");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .HasColumnName("userName");

            entity.HasOne(d => d.Branch).WithMany(p => p.Users)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FKUser135985");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKUser635730");
        });

        modelBuilder.Entity<UserDetail>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserDeta__CB9A1CDF3FA913B5");

            entity.ToTable("UserDetail");

            entity.Property(e => e.UserId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("userID");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Facebook)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("facebook");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("firstName");
            entity.Property(e => e.Img)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("img");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("lastName");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("phone");

            entity.HasOne(d => d.User).WithOne(p => p.UserDetail)
                .HasForeignKey<UserDetail>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKUserDetail940563");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
