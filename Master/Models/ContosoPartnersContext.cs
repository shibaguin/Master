using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Master.Models;

public partial class ContosoPartnersContext : DbContext
{
    public ContosoPartnersContext()
    {
        Log.Debug("Создание нового экземпляра ContosoPartnersContext");
    }

    public ContosoPartnersContext(DbContextOptions<ContosoPartnersContext> options)
        : base(options)
    {
        Log.Debug("Создание нового экземпляра ContosoPartnersContext с опциями");
    }

    public virtual DbSet<MaterialType> MaterialTypes { get; set; }

    public virtual DbSet<Partner> Partners { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductMaterial> ProductMaterials { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    {
        if (!optionsBuilder.IsConfigured)
        {
            Log.Debug("Настройка подключения к базе данных");
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=ContosoPartners;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Log.Debug("Настройка модели данных");
        try
        {
            modelBuilder.Entity<MaterialType>(entity =>
            {
                entity.HasKey(e => e.MaterialId);

                entity.Property(e => e.MaterialId)
                    .HasMaxLength(10)
                    .IsFixedLength()
                    .HasColumnName("MaterialID");
                entity.Property(e => e.MaterialType1)
                    .HasMaxLength(50)
                    .HasColumnName("MaterialType");
                entity.Property(e => e.RejectRate).HasColumnType("decimal(5, 4)");
            });

            modelBuilder.Entity<Partner>(entity =>
            {
                entity.Property(e => e.PartnerId)
                    .HasMaxLength(10)
                    .IsFixedLength()
                    .HasColumnName("PartnerID");
                entity.Property(e => e.Address).HasMaxLength(100);
                entity.Property(e => e.Director).HasMaxLength(100);
                entity.Property(e => e.Inn)
                    .HasMaxLength(12)
                    .HasColumnName("INN");
                entity.Property(e => e.Mail).HasMaxLength(50);
                entity.Property(e => e.PartnerName).HasMaxLength(50);
                entity.Property(e => e.PartnerType).HasMaxLength(50);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Rating)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductId)
                    .HasMaxLength(10)
                    .IsFixedLength()
                    .HasColumnName("ProductID");
                entity.Property(e => e.ProductName).HasMaxLength(50);
                entity.Property(e => e.ProductTypeId)
                    .HasMaxLength(10)
                    .IsFixedLength()
                    .HasColumnName("ProductTypeID");
            });

            modelBuilder.Entity<ProductMaterial>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.MaterialId });

                entity.Property(e => e.ProductId)
                    .HasMaxLength(10)
                    .IsFixedLength()
                    .HasColumnName("ProductID");
                entity.Property(e => e.MaterialId)
                    .HasMaxLength(10)
                    .IsFixedLength()
                    .HasColumnName("MaterialID");
                entity.Property(e => e.QuantityRequired).HasColumnType("decimal(10, 2)");
            });

            modelBuilder.Entity<ProductType>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ProductFactor).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.ProductType1)
                    .HasMaxLength(50)
                    .HasColumnName("ProductType");
                entity.Property(e => e.ProductTypeId)
                    .HasMaxLength(10)
                    .IsFixedLength()
                    .HasColumnName("ProductTypeID");
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.Property(e => e.SaleId)
                    .HasMaxLength(10)
                    .IsFixedLength()
                    .HasColumnName("SaleID");
                entity.Property(e => e.PartnerId)
                    .HasMaxLength(10)
                    .IsFixedLength()
                    .HasColumnName("PartnerID");
                entity.Property(e => e.ProductId)
                    .HasMaxLength(10)
                    .IsFixedLength()
                    .HasColumnName("ProductID");
            });

            Log.Information("Модель данных успешно настроена");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка при настройке модели данных");
            throw;
        }
    }

    public override int SaveChanges()
    {
        try
        {
            Log.Debug("Сохранение изменений в базе данных");
            var result = base.SaveChanges();
            Log.Information("Изменения успешно сохранены в базе данных");
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка при сохранении изменений в базе данных");
            throw;
        }
    }
}
