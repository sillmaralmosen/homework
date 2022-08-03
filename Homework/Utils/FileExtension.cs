using System.Net;
using System.Text;

namespace Homework.Utils;

public static class FileExtension
{
    public static IFormFile LoadFromUrl(string url)
    {
        byte[] response;
        response = new WebClient().DownloadData(url);
        return ConvertFromByte(response);
    }

    public static IFormFile LoadFromPath (string path)
    {
        using FileStream sourceStream = File.Open(path, FileMode.Open);
        var reader = new StreamReader(sourceStream);
            
        byte[] byteArray = Encoding.ASCII.GetBytes(reader.ReadToEnd());
        return ConvertFromByte(byteArray, Path.GetFileName(path), path.Split(@"\").Last());
    }

    private  static IFormFile ConvertFromByte(byte[] byteArray, string filename = "download", string path = "download")
    {
        using Stream stream = new MemoryStream(byteArray);
        return new FormFile(stream, 0, stream.Length, filename, path);
    }
}