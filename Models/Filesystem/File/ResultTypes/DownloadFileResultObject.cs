using Microsoft.AspNetCore.Mvc;

namespace Tessa.Models.Filesystem.File.ResultTypes;

public class DownloadFileResultObject
{
    public FileStreamResult? Value { get; set; }
    public DownloadFileResultType Code { get; set; }
}