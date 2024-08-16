using System.Diagnostics.CodeAnalysis;

namespace GarageCoreMVC.Common.Utilities
{
    [ExcludeFromCodeCoverage]
    public static class Uploader
    {
        public static async Task UploadAsync(IFormFile file, string path)
        {
            if (file != null && file.Length > 0)
            {
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), path, file.FileName);
                Console.WriteLine(Directory.GetCurrentDirectory());
                using (var stream = System.IO.File.OpenWrite(uploadPath))
                {
                    await file.CopyToAsync(stream);
                }
                Console.WriteLine($"Successfully upload {file.FileName} !!!");
            }
            Console.WriteLine($"Failed to upload file !!!");
        }
    }
}
