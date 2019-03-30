using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using RealEstate.Base;
using RealEstate.Extensions.FileHandler;
using RealEstate.Extensions.FileHandler.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using System;
using System.IO;

namespace RealEstate.Services.Base
{
    public interface IFileHandler
    {
        ExistingFile Exist(string directory, string fileName, string extension);

        bool DeleteFilePermanently(string fileName);

        RockFile Upload(IFormFile file);
    }

    public class FileHandler : IFileHandler
    {
        private readonly IHostingEnvironment _environment;

        public FileHandler(
            IHostingEnvironment environment
            )
        {
            _environment = environment;
        }

        public bool DeleteFilePermanently(string fileName)
        {
            try
            {
                var file = _environment.MapPath(Path.Combine("docs\\", fileName));
                if (!File.Exists(file)) return false;
                File.Delete(file);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ExistingFile Exist(string directory, string fileName, string extension)
        {
            directory = _environment.MapPath(directory);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            var newFileName = fileName.SanitizeFileName();
            var existCheckDone = false;
            var index = 2;
            do
            {
                if (File.Exists(Path.Combine(directory, $"{newFileName}.{extension}")))
                {
                    newFileName = $"{fileName}_{index}";
                    index++;
                }
                else
                {
                    existCheckDone = true;
                }
            } while (!existCheckDone);

            return new ExistingFile
            {
                Directory = directory,
                FileExtension = extension,
                FileName = newFileName
            };
        }

        public RockFile Upload(IFormFile file)
        {
            if (file == null || file.Length <= 0)
                return null;

            const string directory = "docs\\";

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

            var reservedFilename =
                $"{DateTime.Now.ToString("s").Replace("-", "").Replace(":", "").Replace("T", "")}";

            using (var image = Image.Load(memoryStream2Bytes))
                image.SaveAsJpeg(outputStream, jpgEncoder);

            var finalFilename = Exist(directory, reservedFilename, "jpg");
            if (finalFilename == null) return null;

            outputStream.Seek(0, SeekOrigin.Begin);
            try
            {
                var finalFilePath = Path.Combine(finalFilename.Directory,
                    $"{finalFilename.FileName}.{finalFilename.FileExtension}");
                outputStream.SaveAs(finalFilePath, stream =>
                {
                    outputStream.Flush();
                });

                return new RockFile
                {
                    File = $"{finalFilename.FileName}.{finalFilename.FileExtension}",
                    Extension = finalFilename.FileExtension,
                    Name = finalFilename.FileName,
                };
            }
            catch
            {
                return null;
            }
        }
    }
}