using System.Diagnostics;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Net.Http.Headers;
using Tessa.Models.Filesystem.Directory;
using Tessa.Models.Filesystem.Directory.ResultTypes;
using Tessa.Models.Filesystem.File.ResultTypes;
using Tessa.Models.User;
using Tessa.Persistance.PostgreSQL;
using Tessa.Services.DriveManager;
using Tessa.Utilities.Configuration;
using Tessa.Utilities.PathHelper;
using Tessa.Utilities.Result;

namespace Tessa.Models.Filesystem.File;

public class FileService : IFileService
{
    private readonly TessaDbContext _context;
    private readonly IDriveManager _driveManager;
    private readonly IUserService _userService;
    private readonly IDirectoryService _directoryService;
    private readonly IConfigurationService _configurationService;
    public FileService(
        TessaDbContext context, 
        IDriveManager driveManager, 
        IUserService userService, 
        IDirectoryService directoryService,
        IConfigurationService configurationService
    )
    {
        _context = context;
        _driveManager = driveManager;
        _userService = userService;
        _directoryService = directoryService;
        _configurationService = configurationService;
    }

    /// <summary>
    /// Uploads all selected files to the specified path. <br/>
    /// Most upload logic is handled by the HandleOneItem method.
    /// </summary>
    /// <param name="uploadObject"></param>
    public async Task<Result<UploadFileResultObject>> UploadMany(UploadInputWrapper uploadObject)
    {
        User.User? user = await _userService.GetUserFromHttpContext();
        if (user == null)
            return new Result<UploadFileResultObject>(
                new UploadFileResultObject()
                {
                    Code = UploadFileResultType.NotLogged,
                    SuccessfulUploads = null
                }
            );

        Directory.Directory fetched;
        string path = uploadObject.Path.NormalizeDirectoryPath(user.Username!);
        try
        {
            fetched = await _context.Items
                .OfType<Directory.Directory>()
                .Where(e => e.Path == path)
                .Include(e => e.Children)
                .AsNoTracking()
                .FirstAsync();
        }
        catch (Exception)
        {
            return new Result<UploadFileResultObject>(
                new UploadFileResultObject()
                {
                    Code = UploadFileResultType.ParentInvalid,
                    SuccessfulUploads = null
                });
        }
        
        if (fetched.OwnerId != user.Id) return new Result<UploadFileResultObject>(
            new UploadFileResultObject()
            {
                Code = UploadFileResultType.NotOwner,
                SuccessfulUploads = null
            }
        );

        UploadFileResultObject result = new ()
        {
            Code = UploadFileResultType.Ok,
            SuccessfulUploads = new List<OneItemWrapper>()
        };
        bool _result;
        foreach (IBrowserFile file in uploadObject.Files)
        {
            _result = await HandleOneItem(file, fetched);
            if (_result)
            {
                result.SuccessfulUploads.Add(
                    new OneItemWrapper()
                    {
                        Name = file.Name,
                        Result = true
                    });
            }
            else
            {
                result.SuccessfulUploads.Add(
                    new OneItemWrapper()
                    {
                        Name = file.Name,
                        Result = false
                    });
                result.Code = UploadFileResultType.Incomplete;
            }
        }
        return new Result<UploadFileResultObject>(result);
    }

    /// <summary>
    /// Ensures the file entry is unique in the database and creates an entry.<br/>
    /// Then it handles file upload to the physical drive.
    /// </summary>
    /// <param name="file">File to save</param>
    /// <param name="parent">Parent folder reference for database update</param>
    /// <returns></returns>
    private async Task<bool> HandleOneItem(IBrowserFile file, Directory.Directory parent)
    {
        File newEntry = new ()
        {
            Path = $"{parent.Path}{file.Name}",
            Name = file.Name,
            OwnerId = parent.OwnerId,
            ParentId = parent.Id
        };
        string? ext = newEntry.Name.GetFileExtension();
        if (ext == null) return false;
        else newEntry.Extension = ext;

        if (parent.Children!.Any(i => i.Name == file.Name)) return false;
        _context.Items.Add(newEntry);
        
        try
        {
            FileStream? physicalFileStream = _driveManager.CreateFile(newEntry.Path);
            if (physicalFileStream == null) return false;
            Stream browserStream = file.OpenReadStream(1024*1024*200);
            await browserStream.CopyToAsync(physicalFileStream, 1024 * 1024);
            await physicalFileStream.DisposeAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _context.Items.Remove(newEntry);
            await _context.SaveChangesAsync();
            return false;
        }
        
        await _context.SaveChangesAsync();
        return true;
    }

    
    public async Task<Result<RenameFileResultType>> RenameAsync(string oldPath, string newPath)
    {
        File file = await _context.Items
            .OfType<File>()
            .Where(e => e.Path == oldPath)
            .FirstAsync();

        if (file == null) return new Result<RenameFileResultType>(RenameFileResultType.InvalidFile);
        
        string tempParentPath = newPath.GetDirectoryParentPath();
        Guid? parent = await _directoryService.GetGuidAsync(tempParentPath);
        if (parent == null) return new Result<RenameFileResultType>(RenameFileResultType.InvalidParent);
        
        file.Path = newPath;
        file.Name = newPath.ExtractFilename();
        file.Extension = file.Name.GetFileExtension();
        file.ParentId = parent.Value;
        
        try
        {
            _driveManager.MoveFile(oldPath, newPath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new Result<RenameFileResultType>(RenameFileResultType.DriveError);
        }
        
        await _context.SaveChangesAsync();
        return new Result<RenameFileResultType>(RenameFileResultType.Ok);
    }
    
    public async Task<Result<DeleteFileResultType>> DeleteAsync(string path)
    {
        File fetched = await _context.Items
            .OfType<File>()
            .FirstAsync(e => e.Path == path);
        if (fetched == null) return new Result<DeleteFileResultType>(DeleteFileResultType.InvalidFile);

        User.User? user = await _userService.GetUserFromHttpContext();
        if (user == null) return new Result<DeleteFileResultType>(DeleteFileResultType.NotLogged);

        if (fetched.OwnerId != user.Id) return new Result<DeleteFileResultType>(DeleteFileResultType.NotOwner);

        if (!_driveManager.DeleteFile(path)) return new Result<DeleteFileResultType>(DeleteFileResultType.DriveError);
        
        _context.Items.Remove(fetched);
        await _context.SaveChangesAsync();
        return new Result<DeleteFileResultType>(DeleteFileResultType.Ok);
    }

    public async Task<Result<DownloadFileResultObject>> Download(string path)
    {
        if (string.IsNullOrEmpty(path) || path == "/")
        {
            return new Result<DownloadFileResultObject>(
                new DownloadFileResultObject()
                {
                    Code = DownloadFileResultType.PathInvalid
                }
            );
        }
        
        User.User? user = await _userService.GetUserFromHttpContext();
        if (user == null)
        {
            return new Result<DownloadFileResultObject>(
                new DownloadFileResultObject()
                {
                    Code = DownloadFileResultType.Authentication
                }
            );
        }
        
        string qualifiedPath = FilePathHelper.NormalizeFileFromDownloadLink(_configurationService.Configuration.Root, user.Username!, path);
        FileStream file;
        FileInfo info = new FileInfo(path);
        try
        {
            file = new(qualifiedPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        catch (Exception)
        {
            return new Result<DownloadFileResultObject>(
                new DownloadFileResultObject()
                {
                    Code = DownloadFileResultType.PathInvalid,
                    Value = null
                });
        }

        return new Result<DownloadFileResultObject>(
            new DownloadFileResultObject()
            {
                Code = DownloadFileResultType.Ok,
                Value = new FileStreamResult(file, "application/octet-stream")
                {
                    EnableRangeProcessing = true,
                    FileDownloadName = info.Name
                }
            }
        );
    }
}