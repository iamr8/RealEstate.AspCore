using System;
using System.IO;
using System.Threading.Tasks;

namespace RealEstate.Services.Extensions
{
    public static class FileExtensions
    {
        public static async Task<bool> SaveAs<T>(this T file, string path, Action<T> action) where T : Stream
        {
            if (file.Length <= 0) return false;

            try
            {
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    action(file);
                    fileStream.Flush();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}