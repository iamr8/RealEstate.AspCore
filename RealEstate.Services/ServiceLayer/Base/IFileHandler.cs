using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using RealEstate.Base;
using RealEstate.Services.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace RealEstate.Services.ServiceLayer.Base
{
    public interface IFileHandler
    {
        bool Delete(string fileName);

        Task<List<string>> SaveAsync(IFormFile[] files);

        Task<string> SaveAsync(IFormFile file);
    }

    public class FileHandler : IFileHandler
    {
        private readonly IHostingEnvironment _environment;
        public const string ImgDirectory = "img";

        public FileHandler(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public bool Delete(string fileName)
        {
            try
            {
                var file = _environment.MapPath(Path.Combine(ImgDirectory, fileName));
                if (!File.Exists(file)) return false;
                File.Delete(file);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> SaveAsync(IFormFile file)
        {
            var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            memoryStream.Position = 0;
            memoryStream.Seek(0, SeekOrigin.Begin);
            var memoryStream2Bytes = memoryStream.ToArray();
            if (memoryStream2Bytes.Length == 0) return null;

            var outputStream = new MemoryStream();
            var jpgEncoder = new JpegEncoder { Quality = 70 };

            var format = Image.DetectFormat(memoryStream2Bytes);
            if (format == null) return null;

            var toDate = DateTime.Now;
            var persianC = new PersianCalendar();
            var year = persianC.GetYear(toDate);
            var month = persianC.GetMonth(toDate);
            var day = persianC.GetDayOfMonth(toDate);

            var targetDir = $"{year}/{month}/{day}";
            DirectoryCheck($"{ImgDirectory}/{targetDir}");

            using (var image = Image.Load(memoryStream2Bytes))
                image.SaveAsJpeg(outputStream, jpgEncoder);

            outputStream.Seek(0, SeekOrigin.Begin);
            try
            {
                var fileName = $"{Guid.NewGuid()}.jpg";
                var filePath = $"{targetDir}/{fileName}";
                var finalFilePath = _environment.MapPath($"{ImgDirectory}/{filePath}");
                var saved = await outputStream.SaveAs(finalFilePath, stream =>
                {
                    outputStream.Flush();
                }).ConfigureAwait(false);
                return saved ? filePath : null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<string>> SaveAsync(IFormFile[] files)
        {
            if (files?.Any() != true)
                return default;

            var validFiles = files.Where(x => x.Length > 0).ToList();
            if (validFiles?.Any() != true)
                return default;

            var list = new List<string>();
            foreach (var validFile in validFiles)
            {
                var file = await SaveAsync(validFile).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(file))
                    list.Add(file);
            }

            var result = list.Where(x => !string.IsNullOrEmpty(x)).ToList();
            return result;
        }

        private void DirectoryCheck(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException(path);

            var result = new List<DirectoryInfo>();
            var dirArrays = path.Split("\\");
            for (var i = 0; i < dirArrays.Length; i++)
            {
                var thisPath = _environment.MapPath(string.Join("\\", dirArrays.Take(i + 1)));
                var directory = new DirectoryInfo(thisPath);
                if (!directory.Exists)
                    directory.Create();

                result.Add(directory);
            }
        }
    }
}