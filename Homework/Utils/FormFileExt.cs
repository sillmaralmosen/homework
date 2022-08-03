namespace Homework.Utils;

public static class FormFileExt
{
    public static async Task<byte[]> GetBytesAsync(this IFormFile formFile)
    {
        if (formFile == null || formFile.Length <= 0) return null;

        await using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}