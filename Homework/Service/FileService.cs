using System.Text;
using System.Xml.Serialization;
using Homework.Enums;
using Homework.Interface;
using Homework.Model;
using Homework.Model.Request;
using Homework.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Homework.Services;

public class FileService : IFileService
{
    #region Implementation

    public async Task<FileContentResult> Download(DownloadFileRequest request)
    {
        var requestFile = Read(request);
        var doc = await XmlRead(requestFile);
        var export = SerializeObject(doc, request.ExtensionExport);

        var bytes = Encoding.UTF8.GetBytes(export);
        var output = new FileContentResult(bytes, "application/octet-stream");

        output.FileDownloadName =
            Path.GetFileNameWithoutExtension(requestFile.FileName) + "." + request.ExtensionExport;

        return output;
    }

    public async Task Send(SendFileRequest request)
    {
        var requestFile = Read(request);

        var doc = await XmlRead(requestFile);
        var export = SerializeObject(doc, request.ExtensionExport);

        if (EmailValidator.IsValidEmail(request.ExportPathEmail))
        {
            // odesílání mailů ToDo 
        }

        if (PathExtension.IsLocalPath(request.ExportPathEmail))
            await File.WriteAllTextAsync(request.ExportPathEmail, export);
    }

    #endregion

    #region Private Methods

    private string SerializeObject(Document input, ExportExtension type)
    {
        switch (type)
        {
            case ExportExtension.json:
                return JsonConvert.SerializeObject(input);
            case ExportExtension.proto3:
                return ProtoSerializer.ProtoSerialize(input);
            default:
                return null;
        }
    }

    private async Task<Document> XmlRead(IFormFile file)
    {
        XmlValidator.XmlByXsd(await file.GetBytesAsync(), "validace - schema", "validace.xsd");

        var serializer = new XmlSerializer(typeof(Document));
        var documentResponse = serializer.Deserialize(file.OpenReadStream());

        return (Document)documentResponse!;
    }


    private IFormFile Read(DownloadFileRequest request)
    {
        if (request.File != null)
            return request.File;
        if (request.File == null && !string.IsNullOrWhiteSpace(request.ImportPathUrl) &&
            PathExtension.IsLocalPath(request.ImportPathUrl))
            return FileExtension.LoadFromPath(request.ImportPathUrl);
        if (request.File == null && !string.IsNullOrWhiteSpace(request.ImportPathUrl) &&
            !PathExtension.IsLocalPath(request.ImportPathUrl))
            return FileExtension.LoadFromUrl(request.ImportPathUrl);
        return null;
    }

    #endregion
}