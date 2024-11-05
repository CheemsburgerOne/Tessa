using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Tessa.Models.Filesystem.File;
using Tessa.Models.Filesystem.File.ResultTypes;

namespace Tessa.Controllers.Download
{

    [ApiController]
    [Route("download")]
    public class DownloadController : ControllerBase
    {
        private readonly IFileService _fileService;

        public DownloadController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet("{path}")]
        public async Task<IActionResult> Download(string path)
        {
            if ((await _fileService.Download(path)).TryGetResult(out DownloadFileResultObject? result))
            {
                if (result!.Code == DownloadFileResultType.Ok) return result.Value!;
                if (result!.Code == DownloadFileResultType.Authentication) return Unauthorized();
            }
            return BadRequest();
        }
    }
}
