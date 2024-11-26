namespace Ab108Uniqlo.Extensions;

public static class FileExtensions
{
    public static bool IsValidType(this IFormFile file, string type)
        => file.ContentType.StartsWith(type);

    public static bool IsValidSize(this IFormFile file, int kb)
    {
        return file.Length <= kb * 1024 * 1024;
    }
    public static async Task<string> UploadAsync(this IFormFile file, string filename)
    {
        if (Path.Exists(filename))
        {
            Directory.CreateDirectory(filename);
        }
        string newfilename = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
        using (Stream st = File.Create(Path.Combine(filename, newfilename)))
        {
            await file.CopyToAsync(st);
        }
        return newfilename;
    }
}
