using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DepremSafe.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DepremSafe.Data.Context
{
    public class DepremSafeDbContext : DbContext
    {
        // Runtime için DI constructor
        public DepremSafeDbContext(DbContextOptions<DepremSafeDbContext> options)
            : base(options) { }

        // Design-time ve factory gerekmez
        public DepremSafeDbContext() { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserLocation> UserLocations { get; set; }
        public DbSet<Earthquake> Earthquakes { get; set; }
        public DbSet<City> Cities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Design-time veya parameterless constructor kullanıldığında çalışır
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=DepremSafeDB;Trusted_Connection=True;");

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tabloların adını özelleştir
            modelBuilder.Entity<User>().ToTable("tbl_User");
            modelBuilder.Entity<UserLocation>().ToTable("tbl_UserLocation");
            modelBuilder.Entity<Earthquake>().ToTable("tbl_Earthquake");

            // User -> UserLocation ilişkisi
            modelBuilder.Entity<User>()
                .HasMany(u => u.Locations)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Alan konfigürasyonları
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            modelBuilder.Entity<Earthquake>()
                .HasIndex(e => e.EventId)
                .IsUnique();
        }
    }
    }
