using Microsoft.AspNetCore.Mvc;
using Notino.Model.Request;

namespace Notino.Interface
{
    public interface IFileService
    {
        Task<FileContentResult> Download(DownloadFileRequest request);
        Task Send(SendFileRequest request);
    }
}
