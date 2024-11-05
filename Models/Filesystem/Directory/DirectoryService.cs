using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tessa.Models.Filesystem.Directory.ResultTypes;
using Tessa.Models.Filesystem.File;
using Tessa.Models.User;
using Tessa.Persistance.PostgreSQL;
using Tessa.Services.DriveManager;
using Tessa.Utilities.Configuration;
using Tessa.Utilities.IconManager;
using Tessa.Utilities.PathHelper;
using Tessa.Utilities.Result;

namespace Tessa.Models.Filesystem.Directory;

public class DirectoryService : IDirectoryService
{
    private readonly TessaDbContext _context;
    private readonly IDriveManager _driveManager;
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _httpContext;
    private readonly IIconManager _iconManager;
    
    public DirectoryService(
        TessaDbContext context,
        IDriveManager driveManager,
        IUserService userService,
        IHttpContextAccessor contextAccessor,
        IIconManager iconManager
    ) {
        _context = context;
        _driveManager = driveManager;
        _userService = userService;
        _httpContext = contextAccessor;
        _iconManager = iconManager;
        // _eventService = eventService;
    }
    /// <summary>
    /// Creates folder for the calling user on the drives and add entry in the database. <br/>
    /// Use CreateUserRoot to register a new user as it does not check parent ownership constraint etc. 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<Result<CreateDirectoryResultType>> CreateAsync(DirectoryEditDto dto)
    {
        User.User? user = await _userService.GetUserFromHttpContext();
        if (user == null)
            return new Result<CreateDirectoryResultType>(
                CreateDirectoryResultType.NotLogged
            );

        if (string.IsNullOrEmpty(dto.Name) || dto.Path == null)
        {
            return new Result<CreateDirectoryResultType>(
                CreateDirectoryResultType.DataInvalid
            );
        }
        
        // Normalize and validate input
        if (dto.Name.NameHasValidChars() && dto.Path.PathHasValidChars())
        {
            dto.Name = dto.Name;
            dto.Path = dto.Path.NormalizeDirectoryPath(user.Username!);
        }
        else return new Result<CreateDirectoryResultType>(CreateDirectoryResultType.DataInvalid);

        Directory? parent;
        try
        {
            parent = await _context.Items.OfType<Directory>().Where(e => e.Path == dto.Path).FirstAsync();
        }
        catch (Exception)
        {
            return new Result<CreateDirectoryResultType>(CreateDirectoryResultType.ParentInvalid);
        }
        
        //TODO! add checks. only admin can create in root. 
        if (parent.OwnerId != user.Id) return new Result<CreateDirectoryResultType>(CreateDirectoryResultType.NotOwner);
        
        Directory entry = new Directory()
        {
            OwnerId = user.Id,
            ParentId = parent.Id,
            Path = $"{dto.Path}{dto.Name}/",
            Name = dto.Name,
        };

        if (await _context.Items.AnyAsync(e => e.Path == entry.Path))
            return new Result<CreateDirectoryResultType>(CreateDirectoryResultType.Duplicate);
        
        EntityEntry<Base.Base> added = _context.Items.Add(entry);
        DirectoryInfo? dirInfo = _driveManager.CreateDirectory(entry.Path);
        
        if (dirInfo == null)
        {
            _context.Items.Remove(added.Entity);
            await _context.SaveChangesAsync();
            return new Result<CreateDirectoryResultType>(CreateDirectoryResultType.DriveError);
        }
        
        await _context.SaveChangesAsync();
        return new Result<CreateDirectoryResultType>(CreateDirectoryResultType.Ok);
    }

    public async Task CreateUserRoot(string username)
    {
        
    }
    
    public async Task<Guid?> GetGuidAsync(string path)
    {
        User.User? user = await _userService.GetUserFromHttpContext();
        if (path.PathHasValidChars())
        {
            path = path.NormalizeDirectoryPath(user!.Username);
        }
        //TODO! Refactor when debugging is done.
        Directory? fetched = 
            await _context.Items
                .OfType<Directory>()
                .Where(e => e.Path == path)
                .AsNoTracking()
                .FirstAsync();

        if (fetched == null) return null;
        return fetched.Id;
    }

    public Task<bool> MoveAsync(string path, string newName)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RenameAsync(string path, string newName)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<DeleteDirectoryResultType>> DeleteAsync(string path)
    {
        Directory? fetched = await _context.Items
            .OfType<Directory>()
            .FirstAsync(e => e.Path == path);

        if (fetched == null) 
            return new Result<DeleteDirectoryResultType>(DeleteDirectoryResultType.NotFound);
        
        User.User? user = await _userService.GetUserFromHttpContext();
        if (user == null)
            return new Result<DeleteDirectoryResultType>(DeleteDirectoryResultType.NotOwner);
        
        if (fetched.OwnerId != user.Id)
            return new Result<DeleteDirectoryResultType>(DeleteDirectoryResultType.NotOwner);
        
        if (!_driveManager.DeleteDirectory(path))
            return new Result<DeleteDirectoryResultType>(DeleteDirectoryResultType.DriveError);

        _context.Items.Remove(fetched);
        await _context.SaveChangesAsync();
        return new Result<DeleteDirectoryResultType>(DeleteDirectoryResultType.Ok);
    }

    /// <summary>
    /// Gets the batched directory with all the information for HTTP reply.
    /// </summary>
    /// <param name="path">Path to the directory</param>
    /// <returns>Object containing all the information. Null, if path is invalid or unauthorized access was detected</returns>
    public async Task<Result<GetBatchedResultObject>> GetBatchedAsync(string path)
    {
        User.User? user = await _userService.GetUserFromHttpContext();
        if (user == null)             
            return new Result<GetBatchedResultObject>(
                new GetBatchedResultObject()
                {
                    Code = GetBatchedResultType.NotLogged,
                    Value = null
                }
            );

        path = path.NormalizeDirectoryPath(user.Username!);
        Directory fetched;
        try
        {
            fetched = await _context.Items
                .OfType<Directory>()
                .Where(e => e.Path == path)
                .Include(e => e.Children)
                .AsNoTracking()
                .FirstAsync();
        }
        catch (Exception)
        {
            return new Result<GetBatchedResultObject>(
                new GetBatchedResultObject()
                {
                    Code = GetBatchedResultType.RawNull,
                    Value = null
                });
        }
        
        if (fetched.OwnerId != user.Id)             
            return new Result<GetBatchedResultObject>(
                new GetBatchedResultObject()
                {
                    Code = GetBatchedResultType.NotOwner,
                    Value = null
                }
            );
        
        DirectoryBatchedDto batched = new DirectoryBatchedDto();
        batched.Owner = _httpContext.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        batched.Path = fetched.Path!;
        batched.Name = fetched.Name!;
        batched.ChildCount = (ushort) fetched.Children!.Count;
        batched.Directories = new List<DirectoryMiniDto>();
        batched.Files = new List<FileMiniDto>();
        
        IEnumerable<Directory> directories = fetched.Children.OfType<Directory>();

        if (directories.Any()) batched.DirectoryIconBase64 = _iconManager.GetIcon("dir").FullBase64;
        
        foreach (Directory child in directories)
        {
            batched.Directories.Add(new DirectoryMiniDto()
            {
                Path = child.Path!,
                Name = child.Name!,
            });
        }
        
        foreach (File.File child in fetched.Children.OfType<File.File>())
        {
            batched.Files.Add(new FileMiniDto()
            {
                Name = child.Name!,
                Path = child.Path!,
                Extension = child.Extension,
                IconBase64 = _iconManager.GetIcon(child.Extension).FullBase64
            });
        }
        
        return new Result<GetBatchedResultObject>(
            new GetBatchedResultObject()
            {
                Code = GetBatchedResultType.Ok,
                Value = batched
            }
        );
    }
}