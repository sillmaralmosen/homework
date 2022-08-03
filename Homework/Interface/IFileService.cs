using Homework.Model.Request;
using Microsoft.AspNetCore.Mvc;

namespace Homework.Interface
{
    public interface IFileService
    {
        Task<FileContentResult> Download(DownloadFileRequest request);
        Task Send(SendFileRequest request);
    }
}
