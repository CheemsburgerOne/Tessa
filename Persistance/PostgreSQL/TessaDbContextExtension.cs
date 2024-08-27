using Tessa.Models.User;

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
        
        // context.Directories.Add(new Directory()
        // {
        //     Name = "root",
        //     Path = "/",
        //     OwnerId = null,
        //     ParentId = null,
        // });
        
        context.SaveChanges();
        return context;
    }
    
}