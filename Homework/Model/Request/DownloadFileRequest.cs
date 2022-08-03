using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Homework.Enums;
using Newtonsoft.Json.Converters;

namespace Homework.Model.Request;

public class DownloadFileRequest
{
    [FileExtensions(Extensions = "xml")] public IFormFile? File { get; set; }

    public string? ImportPathUrl { get; set; }

    [Required]
    [EnumDataType(typeof(ExportExtension))]
    [JsonConverter(typeof(StringEnumConverter))]
    public ExportExtension ExtensionExport { get; set; }
}