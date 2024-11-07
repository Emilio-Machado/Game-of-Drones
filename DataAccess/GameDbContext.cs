using Game_of_Drones.DataAccess.Models;
using Game_of_Drones.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Game_of_Drones.DataAccess
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<Move> Moves { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configura el índice único para la columna Name en Player
            modelBuilder.Entity<Player>()
                .HasIndex(p => p.Name)
                .IsUnique();

            // Configura la relación de autorreferencia en Move para KillMoveId
            modelBuilder.Entity<Move>()
                .HasOne<Move>()
                .WithMany()
                .HasForeignKey(m => m.KillMoveId)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada

            modelBuilder.Entity<Round>()
                .HasOne<Game>()
                .WithMany()
                .HasForeignKey(r => r.GameId)
                .OnDelete(DeleteBehavior.Cascade); // Elimina las rondas cuando se elimina un juego

            // Configura la relación entre Round y Move para PlayerOneMoveId
            modelBuilder.Entity<Round>()
                .HasOne<Move>()
                .WithMany()
                .HasForeignKey(r => r.PlayerOneMoveId)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada de movimientos

            // Configura la relación entre Round y Move para PlayerTwoMoveId
            modelBuilder.Entity<Round>()
                .HasOne<Move>()
                .WithMany()
                .HasForeignKey(r => r.PlayerTwoMoveId)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada de movimientos

            // Configura la relación entre Round y Player para WinnerId
            modelBuilder.Entity<Round>()
                .HasOne<Player>()
                .WithMany()
                .HasForeignKey(r => r.WinnerId)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada de jugadores

            // Configura la relación entre Game y Player para PlayerOneId
            modelBuilder.Entity<Game>()
                .HasOne<Player>()
                .WithMany()
                .HasForeignKey(g => g.PlayerOneId)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada de jugadores

            // Configura la relación entre Game y Player para PlayerTwoId
            modelBuilder.Entity<Game>()
                .HasOne<Player>()
                .WithMany()
                .HasForeignKey(g => g.PlayerTwoId)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada de jugadores

            // Configura la relación entre Game y Player para WinnerId
            modelBuilder.Entity<Game>()
                .HasOne<Player>()
                .WithMany()
                .HasForeignKey(g => g.WinnerId)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada de jugadores

            // Inserta movimientos con KillMoveId
            modelBuilder.Entity<Move>().HasData(
                new Move { Id = 1, Name = "Piedra", DateCreated = DateTime.Now, IsActive = true },
                new Move { Id = 2, Name = "Papel", DateCreated = DateTime.Now, IsActive = true },
                new Move { Id = 3, Name = "Tijera", DateCreated = DateTime.Now, IsActive = true }
            );
        }
    }
}
