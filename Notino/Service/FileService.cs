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

            output.FileDownloadName =Path.GetFileNameWithoutExtension(requestFile.FileName) + "." + request.ExtensionExport.ToString();

            if (output == null)
                throw new Exception("Chyba sestavení výstupu");

            return output;
        }

        public async Task Send(SendFileRequest request)
        {
            var requestFile = Read(request);

            var doc = await XmlRead(requestFile);
            var export = SerializeObject(doc, request.ExtensionExport);

            if (IsValidEmail(request.ExportPathEmail))
            {
                // odesílání mailů Nástřel 
                MailMessage mail = new MailMessage();
                byte[] byteArray = Encoding.ASCII.GetBytes(export);
                var memStream = new MemoryStream(byteArray);
                memStream.Position = 0;
                var contentType = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
                var attachment = new Attachment(memStream, contentType);
                attachment.ContentDisposition.FileName = "download." + request.ExtensionExport.ToString();
                mail.Attachments.Add(attachment);
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
                    throw new Exception("chyba deserializace");
            }
        }

        private async Task<Document> XmlRead(IFormFile file)
        {
            try
            {
                XmlValidator.XmlByXsd(await file.GetBytesAsync());
            }
            catch (Exception)
            {
                throw new Exception("Vybraný soubor neodpovídá požadované struktuře.");
            }

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
            else
                throw new Exception("chyba načtení xml");

            return requestFile;
        }
        private IFormFile LoadFilefromUrl(string url)
        {
            byte[] response;
            try
            {
                response = new WebClient().DownloadData(url);
            }
            catch (Exception ex)
            {
                throw new Exception("Chyba stahování", ex);
            }

            return ConvertFilefromByte(response);
        }

        private IFormFile LoadFilefromPath (string path)
        {
            string input;
            try
            {
                FileStream sourceStream = File.Open(path, FileMode.Open);
                var reader = new StreamReader(sourceStream);
                input = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new Exception("Chyba načítání", ex);
            }

            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            return ConvertFilefromByte(byteArray, Path.GetFileName(path), path.Split(@"\").Last());
        }

        private IFormFile ConvertFilefromByte(byte[] byteArray, string filename = "download", string path = "download")
        {
            Stream stream = new MemoryStream(byteArray);
            return new FormFile(stream, 0, stream.Length, filename, path);
        }

        private void SaveFile (string serializedObject, string destinationPath)
        {
            try
            { 
                var targetStream = File.Open(destinationPath, FileMode.Create, FileAccess.Write);
                var sw = new StreamWriter(targetStream);
                sw.Write(serializedObject);
            }
            catch (Exception ex)
            {
                throw new Exception("Chyba ukládání", ex);
            }
        }

        private static bool IsLocalPath(string p)
        {
            if (p.StartsWith("http:\\"))
                return false;

            return new Uri(p).IsFile;
        }

        bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
                return false; 

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
