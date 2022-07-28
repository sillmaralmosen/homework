using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;
using Notino.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Notino.Model.Request
{
    public class DownloadFileRequest
    {
        [FileExtensions(Extensions = "xml")]
        public IFormFile? File { get; set; }
        public string? ImportPathUrl { get; set; }

        [Required]
        [EnumDataType(typeof(ExportExtensionEnum))]
        [JsonConverter(typeof(StringEnumConverter))]
        public ExportExtensionEnum Extension { get; set; }
    }
}
