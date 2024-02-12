using BattleShip.models;
using Microsoft.EntityFrameworkCore;

public class ApplicationContext : DbContext
{
    public DbSet<PlayerEntity> Players { get; set; }
    public DbSet<MoveEntity> Moves { get; set; }

    public ApplicationContext()
    {
        Database.EnsureCreated();   
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=database.db");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerEntity>().ToTable("Players");
        modelBuilder.Entity<PlayerEntity>(builder => {
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.Name).IsRequired();
            builder.HasMany(p => p.Moves)
            .WithOne(m => m.Player)
            .HasForeignKey(m => m.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);;
        });
        modelBuilder.Entity<MoveEntity>().ToTable("Moves");
        modelBuilder.Entity<MoveEntity>(builder => {
            builder.Property(m => m.Id).ValueGeneratedOnAdd();
            builder.Property(m => m.Row).IsRequired();
            builder.Property(m => m.Column).IsRequired();
            builder.Property(m => m.Hit).IsRequired();
            builder.Property(m => m.Time).IsRequired();
            builder.HasOne(m => m.Player)
            .WithMany(p => p.Moves)
            .HasForeignKey(m => m.PlayerId);
        });
    }
}