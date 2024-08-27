using Microsoft.EntityFrameworkCore;
using Tessa.Models.User;

namespace Tessa.Persistance.PostgreSQL;

public class TessaDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    // public DbSet<Event> Events { get; set; }
    // public DbSet<Directory> Directories { get; set; }
    // public DbSet<File> Files { get; set; }

    public TessaDbContext()
    {
    }

    public TessaDbContext(DbContextOptions<TessaDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=127.0.0.1;Port=5432;Database=tessa_db;Username=postgres;Password=admin;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //     // User //
        //     modelBuilder.Entity<User>()
        //         .HasMany<Event>( e => e.Events)
        //         .WithOne(e => e.User)
        //         .HasForeignKey(e => e.UserId)
        //         .OnDelete(DeleteBehavior.NoAction);
        //     
        //     // Event //
        //     modelBuilder.Entity<Event>()
        //         .HasOne<User>( e => e.User)
        //         .WithMany( e => e.Events)
        //         .HasForeignKey(e => e.UserId)
        //         .OnDelete(DeleteBehavior.NoAction);
        //     
        //     modelBuilder.Entity<Event>()
        //         .HasOne<Item>( e => e.Item)
        //         .WithMany( e => e.Events)
        //         .HasForeignKey(e => e.ItemId)
        //         .OnDelete(DeleteBehavior.NoAction);
        //     
        //     // Item // 
        //     modelBuilder.Entity<Item>()
        //         .HasOne<User>( e => e.Owner)
        //         .WithMany( e => e.Items)
        //         .HasForeignKey(e => e.OwnerId)
        //         .OnDelete(DeleteBehavior.NoAction);
        //     
        //     modelBuilder.Entity<Item>()
        //         .HasOne<Directory>( e => e.Parent)
        //         .WithMany( e => e.Children)
        //         .HasForeignKey(e => e.ParentId)
        //         .OnDelete(DeleteBehavior.NoAction);
        //     
        //     modelBuilder.Entity<Item>()
        //         .HasMany<Event>( e => e.Events)
        //         .WithOne( e => e.Item)
        //         .HasForeignKey(e => e.ItemId )
        //         .OnDelete(DeleteBehavior.NoAction);
        //     
        //     // General //
        //     modelBuilder.Entity<Item>()
        //         .HasDiscriminator<byte>("Type")
        //         .HasValue<Directory>((byte)0)
        //         .HasValue<File>((byte)1);
    }
}