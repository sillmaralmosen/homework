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
            FileContentResult? output = null;
            var requestFile = Read(request);

            if (requestFile != null)
            {
                var doc = await XmlRead(requestFile);
                var export = SerializeObject(doc, request.Extension);
      
                byte[] bytes = Encoding.UTF8.GetBytes(export);
                output = new FileContentResult(bytes, "application/octet-stream");
                output.FileDownloadName =Path.GetFileNameWithoutExtension(requestFile.FileName)+"."+ request.Extension.ToString();
            }
            else
                throw new Exception("Chyba načtení xml");

            if (output == null)
                throw new Exception("Chyba sestavení výstupu");

            return output;
        }

        public async Task Send(SendFileRequest request)
        {
            var requestFile = Read(request);

            if (requestFile != null)
            {
                var doc = await XmlRead(requestFile);
                var export = SerializeObject(doc, request.Extension);

                if (IsValidEmail(request.ExportPathEmail))
                {
                    // odesílání mailů Nástřel 
                    MailMessage mail = new MailMessage();
                    byte[] byteArray = Encoding.ASCII.GetBytes(export);
                    var memStream = new MemoryStream(byteArray);
                    memStream.Position = 0;
                    var contentType = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
                    var attachment = new Attachment(memStream, contentType);
                    attachment.ContentDisposition.FileName = "download." + request.Extension.ToString();
                    mail.Attachments.Add(attachment);
                }
                if (IsLocalPath(request.ExportPathEmail))
                {
                    File.WriteAllText(request.ExportPathEmail, export);
                }
            }
            else
                throw new Exception("chyba načtení xml");
        }
        #endregion

        #region Private Methods
        private string SerializeObject (Document input, ExportExtensionEnum type)
        {
            if (type== ExportExtensionEnum.json)
                return JsonConvert.SerializeObject(input);
            if (type == ExportExtensionEnum.proto3)
                return ProtoSerializer.ProtoSerialize<Document>(input);
            else
                throw new Exception("chyba deserializace");
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

            return requestFile;
        }
        private IFormFile LoadFilefromUrl(string url)
        {
            byte[] response;
            try
            {
                response = new WebClient().DownloadData(url);
            }
            catch
            {
                throw;
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
            catch
            {
                throw;
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
            catch
            {
                throw;
            }
        }

        private static bool IsLocalPath(string p)
        {
            if (p.StartsWith("http:\\"))
            {
                return false;
            }

            return new Uri(p).IsFile;
        }

        bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; 
            }
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
