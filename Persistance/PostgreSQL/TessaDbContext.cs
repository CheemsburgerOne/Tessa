using Microsoft.EntityFrameworkCore;
using Tessa.Models.Filesystem.Base;
using Tessa.Models.User;
using Tessa.Utilities.Configuration;
using Directory = Tessa.Models.Filesystem.Directory.Directory;
using File = Tessa.Models.Filesystem.File.File;

namespace Tessa.Persistance.PostgreSQL;

public class TessaDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    // public DbSet<Event> Events { get; set; }
    public DbSet<Base> Items { get; set; }
    private string _dbstring;
    public TessaDbContext()
    {
    }

    public TessaDbContext(DbContextOptions<TessaDbContext> options, IConfigurationService configuration) : base(options)
    {
        _dbstring = configuration.Configuration.PostgreSQLString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_dbstring);
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Base>()
            .HasDiscriminator<byte>("Type")
            .HasValue<Directory>((byte)0)
            .HasValue<File>((byte)1);

        modelBuilder.Entity<Base>()
            .HasOne<User>(e => e.Owner)
            .WithMany(e => e.Items)
            .HasForeignKey(e => e.OwnerId)
            .OnDelete(DeleteBehavior.NoAction);
        //     //     
        modelBuilder.Entity<Base>()
            .HasOne<Directory>(e => e.Parent)
            .WithMany(e => e.Children)
            .HasForeignKey(e => e.ParentId)
            .OnDelete(DeleteBehavior.Cascade);

        //     //     // User //
        //     //     modelBuilder.Entity<User>()
        //     //         .HasMany<Event>( e => e.Events)
        //     //         .WithOne(e => e.User)
        //     //         .HasForeignKey(e => e.UserId)
        //     //         .OnDelete(DeleteBehavior.NoAction);
        //     //     
        //     //     // Event //
        //     //     modelBuilder.Entity<Event>()
        //     //         .HasOne<User>( e => e.User)
        //     //         .WithMany( e => e.Events)
        //     //         .HasForeignKey(e => e.UserId)
        //     //         .OnDelete(DeleteBehavior.NoAction);
        //     //     
        //     //     modelBuilder.Entity<Event>()
        //     //         .HasOne<Item>( e => e.Item)
        //     //         .WithMany( e => e.Events)
        //     //         .HasForeignKey(e => e.ItemId)
        //     //         .OnDelete(DeleteBehavior.NoAction);
        //     //     
        //     //     // Item // 
        //     //     
        //     //     modelBuilder.Entity<Item>()
        //     //         .HasMany<Event>( e => e.Events)
        //     //         .WithOne( e => e.Item)
        //     //         .HasForeignKey(e => e.ItemId )
        //     //         .OnDelete(DeleteBehavior.NoAction);
        //     //     
        // General //
        // }
    }
}