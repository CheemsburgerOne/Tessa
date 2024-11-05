using Tessa.Models.User;
using Directory = Tessa.Models.Filesystem.Directory.Directory;

namespace Tessa.Persistance.PostgreSQL;

public static class TessaDbContextExtension
{
    public static TessaDbContext Initialize(this TessaDbContext context)
    {
        context.Database.EnsureCreated();
        if (context.Users.Any()) return context;
        // if (context.Directories.Any()) return context;
        
        context.Users.Add(new User()
        {
            Username = "admin",
            Password = "admin",
            Email = "admin@tessa.com",
            Type = UserType.Admin,
            
        });
        context.SaveChanges();
        
        context.Items.Add(new Directory()
        {
            Name = "root",
            Path = "/",
            OwnerId = context.Users.First(e => e.Username == "admin").Id,
            ParentId = null,
        });
        
        context.SaveChanges();
        return context;
    }
    
}