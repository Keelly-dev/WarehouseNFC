using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WarehouseNFC.Data.Models;

public partial class WarehouseNfcContext : DbContext
{
    public WarehouseNfcContext()
    {
    }

    public WarehouseNfcContext(DbContextOptions<WarehouseNfcContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<NfcTag> NfcTags { get; set; }

    public virtual DbSet<Operation> Operations { get; set; }

    public virtual DbSet<OperationItem> OperationItems { get; set; }

    public virtual DbSet<OperationType> OperationTypes { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductItem> ProductItems { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StoragePosition> StoragePositions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;

        var connectionString = Environment.GetEnvironmentVariable("WAREHOUSE_NFC_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Задайте переменную окружения WAREHOUSE_NFC_CONNECTION_STRING.");

        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF1AF841DDF");

            entity.HasIndex(e => e.Login, "UQ__Employee__5E55825B66493F5F").IsUnique();

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Role).WithMany(p => p.Employees)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employees__RoleI__4D94879B");
        });

        modelBuilder.Entity<NfcTag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK__NfcTags__657CFA4CD026F0E4");

            entity.Property(e => e.TagId)
                .HasMaxLength(100)
                .HasColumnName("TagID");
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.PositionId).HasColumnName("PositionID");

            entity.HasOne(d => d.Item).WithMany(p => p.NfcTags)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NfcTags__ItemID__5AEE82B9");

            entity.HasOne(d => d.Position).WithMany(p => p.NfcTags)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NfcTags__Positio__5BE2A6F2");
        });

        modelBuilder.Entity<Operation>(entity =>
        {
            entity.HasKey(e => e.OperationId).HasName("PK__Operatio__A4F5FC64C67D7281");

            entity.Property(e => e.OperationId).HasColumnName("OperationID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.OperationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OperationTypeId).HasColumnName("OperationTypeID");

            entity.HasOne(d => d.Employee).WithMany(p => p.Operations)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Operation__Emplo__628FA481");

            entity.HasOne(d => d.OperationType).WithMany(p => p.Operations)
                .HasForeignKey(d => d.OperationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Operation__Opera__619B8048");
        });

        modelBuilder.Entity<OperationItem>(entity =>
        {
            entity.HasKey(e => e.OperationItemId).HasName("PK__Operatio__9B290865C8206F2A");

            entity.Property(e => e.OperationItemId).HasColumnName("OperationItemID");
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.OperationId).HasColumnName("OperationID");
            entity.Property(e => e.Result).HasMaxLength(100);

            entity.HasOne(d => d.Item).WithMany(p => p.OperationItems)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Operation__ItemI__66603565");

            entity.HasOne(d => d.Operation).WithMany(p => p.OperationItems)
                .HasForeignKey(d => d.OperationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Operation__Opera__656C112C");
        });

        modelBuilder.Entity<OperationType>(entity =>
        {
            entity.HasKey(e => e.OperationTypeId).HasName("PK__Operatio__FF7FE53300285E7B");

            entity.Property(e => e.OperationTypeId).HasColumnName("OperationTypeID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6ED139C5D0C");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Article).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PackageType).HasMaxLength(50);
            entity.Property(e => e.Unit).HasMaxLength(20);
        });

        modelBuilder.Entity<ProductItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__ProductI__727E83EBBCC3B31B");

            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.Batch).HasMaxLength(50);
            entity.Property(e => e.PositionId).HasColumnName("PositionID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.SerialNumber).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("На складе");

            entity.HasOne(d => d.Position).WithMany(p => p.ProductItems)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductIt__Posit__5812160E");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductIt__Produ__571DF1D5");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3AD86E019E");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<StoragePosition>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PK__StorageP__60BB9A59B2A418D1");

            entity.HasIndex(e => new { e.ShelfNumber, e.ShelfLevel }, "UQ_Position").IsUnique();

            entity.Property(e => e.PositionId).HasColumnName("PositionID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
