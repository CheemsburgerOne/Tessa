using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Tessa.Models.Filesystem.File.ResultTypes;
using Tessa.Utilities.Result;

namespace Tessa.Models.Filesystem.File;

public interface IFileService
{
    /// <summary>
    /// Wrapper function for uploading many files.
    /// </summary>
    /// <param name="uploadObject"></param>
    public Task<Result<UploadFileResultObject>> UploadMany(UploadInputWrapper uploadObject);

    public Task<Result<RenameFileResultType>> RenameAsync(string oldPath, string newPath);
    public Task<Result<DeleteFileResultType>> DeleteAsync(string path);
    public Task<Result<DownloadFileResultObject>> Download(string path);
}
