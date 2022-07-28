using Microsoft.AspNetCore.Mvc;
using Notino.Interface;
using Notino.Model.Request;

namespace Notino.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService=fileService;
        }

        [HttpPost("DownloadFile")]
        public async Task<FileContentResult> Download([FromForm] DownloadFileRequest request) => await _fileService.Download(request);


        [HttpPost("SendFile")]
        public async Task Send([FromForm] SendFileRequest request) => await _fileService.Send(request);


    }
}
