// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using AgilityScore.Models;
using System.IO;

namespace AgilityScore.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Season> Seasons { get; set; }
        public DbSet<EventDay> EventDays { get; set; }
        public DbSet<Competition> Competitions { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Dog> Dogs { get; set; }
        public DbSet<Handler> Handlers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // ✅ Inicializar SQLite
                SQLitePCL.Batteries_V2.Init();

                string dbPath = @"C:\Users\dtc4\Documents\PROJECTS\AgilityScore\agilityscore.db";
                Console.WriteLine("SQLite DB path: " + Path.GetFullPath(dbPath));
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relaciones
            modelBuilder.Entity<EventDay>()
                .HasMany(e => e.Competitions)
                .WithOne(c => c.EventDay)
                .HasForeignKey(c => c.EventDayId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Competition>()
                .HasMany(c => c.Participants)
                .WithOne(p => p.Competition)
                .HasForeignKey(p => p.CompetitionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Dog>()
                .HasMany(d => d.Participants)
                .WithOne(p => p.Dog)
                .HasForeignKey(p => p.DogId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Handler>()
                .HasMany(h => h.Dogs)
                .WithOne(d => d.Handler)
                .HasForeignKey(d => d.HandlerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Índices útiles
            modelBuilder.Entity<Dog>().HasIndex(d => d.Name);
            modelBuilder.Entity<Participant>().HasIndex(p => p.Dorsal);

            // Opcional: evitar duplicados de una misma competición en la misma jornada
            modelBuilder.Entity<Competition>()
                .HasIndex(c => new { c.EventDayId, c.Level, c.Size, c.Type })
                .IsUnique();
        }
    }
}
