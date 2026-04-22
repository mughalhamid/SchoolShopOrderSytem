using Microsoft.EntityFrameworkCore;
using SchoolShopOrderSystem.Domain.Models;
using SchoolShopOrderSystem.Domain.Enums;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Parent> Parents => Set<Parent>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Canteen> Canteens => Set<Canteen>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<IdempotencyRecord> IdempotencyRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Parent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Email)
                .HasMaxLength(200)
                .IsRequired();

            entity.HasIndex(x => x.Email)
                .IsUnique();

            entity.Property(x => x.WalletBalance)
                .HasPrecision(10, 2);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.HasOne(x => x.Parent)
                .WithMany(p => p.Students)
                .HasForeignKey(x => x.ParentId);
        });

        modelBuilder.Entity<Canteen>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Price)
                .HasPrecision(10, 2);

            entity.Property(x => x.AllergenTags)
                .HasMaxLength(500);

            entity.HasOne(x => x.Canteen)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(x => x.CanteenId);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.TotalAmount)
                .HasPrecision(10, 2);

            entity.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(x => x.Parent)
                .WithMany()
                .HasForeignKey(x => x.ParentId);

            entity.HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentId);

            entity.HasOne(x => x.Canteen)
                .WithMany()
                .HasForeignKey(x => x.CanteenId);

            entity.HasMany(x => x.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.UnitPrice)
                .HasPrecision(10, 2);

            entity.HasOne(x => x.MenuItem)
                .WithMany()
                .HasForeignKey(x => x.MenuItemId);
        });


    }
}