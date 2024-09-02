using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tessa.Persistance.PostgreSQL;
using Tessa.Services.DriveManager;
using Tessa.Utilities.PathHelper;

namespace Tessa.Models.Filesystem.Directory;

public class DirectoryService : IDirectoryService
{
    private readonly TessaDbContext _context;
    private readonly IDriveManagerService _driveManagerService;
    // private readonly IEventService _eventService;
    // private EventEditDto _eventDto = new EventEditDto();
    
    public DirectoryService(TessaDbContext context,  IDriveManagerService driveManagerService
        // , IEventService eventService
        )
    {
        _context = context;
        _driveManagerService = driveManagerService;
        // _eventService = eventService;
    }
    
    public async Task<Guid?> CreateAsync(DirectoryEditDto dto)
    {
        // Normalize and validate input
        if (dto.Name!.NameHasValidChars() && dto.Path!.PathHasValidChars())
        {
            dto.Path = dto.Path!.NormalizeDirectoryPath();
            dto.Name = dto.Name!.NormalizeDirectoryName();
        } else return null;
        
        Guid parentId = await _context.Directories.Where(e => e.Path == dto.Path).Select(e => e.Id).FirstOrDefaultAsync();
        if (parentId == Guid.Empty)
        {
            // EventEditDto eventDto = new EventEditDto()
            // {
            //     UserId = null,
            //     EventType = EventType.Mkdir,
            //     ErrorType = ErrorType.ParentDirectoryNotFound,
            //     ItemId = null,  
            //     Description = $"NEW {dto.Name} IN {dto.Wd}",
            // };
            
            // await _eventService.CreateAsync(eventDto);
            // await _context.SaveChangesAsync();
            return null;
        }
        
        Directory entry = new Directory()
        {
            ParentId = parentId,
            Path = $"{dto.Path}/{dto.Name}/",
            Name = dto.Name,
        };

        if (await _context.Directories.AnyAsync(e => e.Path == entry.Path))
        {
            // EventEditDto eventDto = new EventEditDto()
            // {
            //     UserId = null,
            //     EventType = EventType.Mkdir,
            //     ErrorType = ErrorType.DirectoryAlreadyExists,
            //     ItemId = null,
            //     Description = $"NEW {dto.Name} IN {dto.Wd}",
            // };
            
            // await _eventService.CreateAsync(eventDto);
            return null;
        };
        
        EntityEntry<Directory> added = _context.Directories.Add(entry);
        
        // Create directory on a physical drive
        DirectoryInfo? dirInfo = _driveManagerService.CreateDirectory(entry.Path);
        
        if (dirInfo == null)
        {
            // Rollback the database entry and mark event as unsuccessful
            // EventEditDto eventDto = new EventEditDto()
            // {
            //     UserId = null,
            //     EventType = EventType.Mkdir,
            //     ErrorType = ErrorType.FailedToCreatePhysicalDirectoryOnDrive,
            //     ItemId = null,
            //     Description = $"NEW {dto.Name} IN {dto.Wd}",
            // };
            // await _eventService.CreateAsync(eventDto);
            _context.Directories.Remove(added.Entity);
            await _context.SaveChangesAsync();
            return null;
        }
        
        // EventEditDto sucessful = new EventEditDto()
        // {
        //     UserId = null,
        //     EventType = EventType.Mkdir,
        //     ErrorType = ErrorType.Success,
        //     ItemId = added.Entity.Id,
        //     Description = $"NEW {dto.Name} IN {dto.Wd}",
        // };
        
        // await _eventService.CreateAsync(successful);
        await _context.SaveChangesAsync();
        
        return added.Entity.Id;
    }
    
    public async Task<Directory?> GetAsync(string path)
    {
        if (path.PathHasValidChars())
        {
            path = path.NormalizeDirectoryPath();
        }
        return await _context.Directories.Where(e => e.Path == path)
            .Include(e => e.Children)
            .AsNoTracking()
            .FirstAsync();
    }
}