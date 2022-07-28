using System.ComponentModel.DataAnnotations;

namespace Notino.Model.Request
{
    public class SendFileRequest : DownloadFileRequest
    {
        [Required]
        public string ExportPathEmail { get; set; }
    }
}
