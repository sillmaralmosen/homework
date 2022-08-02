using System.Net;
using System.Net.Mail;
using System.Text;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Notino.Enums;
using Notino.Interface;
using Notino.Model;
using Notino.Model.Request;
using Notino.Utils;

namespace Notino.Services
{
    public class FileService : IFileService
    {

        #region Implementation
        public async Task<FileContentResult> Download(DownloadFileRequest request)
        {
            var requestFile = Read(request);
            var doc = await XmlRead(requestFile);
            var export = SerializeObject(doc, request.ExtensionExport);
      
            byte[] bytes = Encoding.UTF8.GetBytes(export);
            var output = new FileContentResult(bytes, "application/octet-stream");

            output.FileDownloadName = Path.GetFileNameWithoutExtension(requestFile.FileName) + "." + request.ExtensionExport.ToString();

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
            if (IsLocalPath(request.ExportPathEmail))
            {
                File.WriteAllText(request.ExportPathEmail, export);
            }
    
        }
        #endregion

        #region Private Methods
        private string SerializeObject (Document input, ExportExtension type)
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


        private IFormFile? Read(DownloadFileRequest request)
        {
            IFormFile? requestFile = null;

            if (request.File != null)
                requestFile = request.File;
            else if (request.File == null && !string.IsNullOrWhiteSpace(request.ImportPathUrl) && IsLocalPath(request.ImportPathUrl))
                requestFile = LoadFilefromPath(request.ImportPathUrl);
            else if (request.File == null && !string.IsNullOrWhiteSpace(request.ImportPathUrl) && !IsLocalPath(request.ImportPathUrl))
                requestFile = LoadFilefromUrl(request.ImportPathUrl);
   
            return requestFile;
        }
        private IFormFile LoadFilefromUrl(string url)
        {
            byte[] response;

            response = new WebClient().DownloadData(url);

            return ConvertFilefromByte(response);
        }

        private IFormFile LoadFilefromPath (string path)
        {
            string input;

            using FileStream sourceStream = File.Open(path, FileMode.Open);
            var reader = new StreamReader(sourceStream);
            input = reader.ReadToEnd();
  
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            return ConvertFilefromByte(byteArray, Path.GetFileName(path), path.Split(@"\").Last());
        }

        private IFormFile ConvertFilefromByte(byte[] byteArray, string filename = "download", string path = "download")
        {
            using Stream stream = new MemoryStream(byteArray);
            return new FormFile(stream, 0, stream.Length, filename, path);
        }

        private static bool IsLocalPath(string p)
        {
            if (p.StartsWith("http:\\"))
                return false;

            return new Uri(p).IsFile;
        }
        #endregion
    }
}
