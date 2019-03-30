using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using RealEstate.Base;
using RealEstate.Extensions.FileHandler.Models;
using SixLabors.ImageSharp;

namespace RealEstate.Extensions.FileHandler
{
    public static class FileProcess
    {
        public static bool IsImage(this MemoryStream imgMemoryStream)
        {
            imgMemoryStream.Position = 0;
            imgMemoryStream.Seek(0, SeekOrigin.Begin);
            if (imgMemoryStream.Length == 0) return false;

            try
            {
                using (var img = Image.Load(imgMemoryStream))
                    return img.Width > 0;
            }
            catch
            {
                return false;
            }
        }

        public static ValidatingFile Validate(this IFormFile file)
        {
            if (file.Length < 0)
            {
                return new ValidatingFile
                {
                    Success = false,
                    File = file
                };
            }

            var extension = Path.GetExtension(file.FileName).Remove(0, 1);
            var requestedExts = new List<string> { "jpg", "png" };

            return requestedExts.Any(x => x == extension)
                ? new ValidatingFile { Success = true, File = file }
                : new ValidatingFile
                {
                    Success = false,
                    File = file
                };
        }

        public static string SanitizeFileName(this string fileName)
        {
            return Regex.Match(fileName.Replace(" ", "_"), RegexPatterns.SafeFilename.Display(),
                RegexOptions.IgnoreCase).Value;
        }

        public static void SaveAs(this IFormFile file, string path)
        {
            if (file.Length <= 0) return;

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }
        }

        public static async void SaveAs<T>(this T file, string path, Action<T> action) where T : Stream
        {
            if (file.Length <= 0) return;

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream).ConfigureAwait(false);
                action.Invoke(file);
                fileStream.Flush();
            }
        }

        public static string Size(this long i)
        {
            var sign = (i < 0 ? "-" : "");
            double readable;
            string suffix;
            if (i >= 0x1000000000000000) // Exabyte
            {
                suffix = "اگزابایت";
                readable = i >> 50;
            }
            else if (i >= 0x4000000000000) // Petabyte
            {
                suffix = "پتابایت";
                readable = i >> 40;
            }
            else if (i >= 0x10000000000) // Terabyte
            {
                suffix = "ترابایت";
                readable = i >> 30;
            }
            else if (i >= 0x40000000) // Gigabyte
            {
                suffix = "گیگابایت";
                readable = i >> 20;
            }
            else if (i >= 0x100000) // Megabyte
            {
                suffix = "مگابایت";
                readable = i >> 10;
            }
            else if (i >= 0x400) // Kilobyte
            {
                suffix = "کیلوبایت";
                readable = i;
            }
            else
            {
                return i.ToString(sign + "0 بایت"); // Byte
            }
            readable /= 1024;
            return sign + readable.ToString("0.### ") + suffix;
        }
    }
}