using System.Net;
using Homework.Interface;
using Homework.Model.Request;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.Swagger.Annotations;

namespace Homework.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("download")]
    [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FileContentResult))]
    public async Task<IActionResult> Download([FromForm] DownloadFileRequest request)
    {
        try
        {
            return Ok(await _fileService.Download(request));
        }
        catch (Exception ex)
        {
            return BadRequest(new { errorMessage = ex.Message });
        }
    }


    [HttpPost("send")]
    public async Task<IActionResult> Send([FromForm] SendFileRequest request)
    {
        try
        {
            await _fileService.Send(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { errorMessage = ex.Message });
        }
    }
}