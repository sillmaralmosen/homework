using System.ComponentModel.DataAnnotations;

namespace Homework.Model.Request;

public class SendFileRequest : DownloadFileRequest
{
    [Required] public string ExportPathEmail { get; set; }
}