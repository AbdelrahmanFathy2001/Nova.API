using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace Talabat.API.Helpers
{
    public class DocumentSettings
    {
        public static string UploadFile(IFormFile file)
        {
            /// 1. Get Located Folder Path
            //var folderPath = @"F:\backTask\test\Demo.PL\Demo.PL\wwwroot\files\images\";
            //var folderPath = Directory.GetCurrentDirectory() + "/wwwroot/files/" + folderName;
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/products");

            /// 2. Get File Name and Make its Name UNIQUE
            var fileName = $"{Guid.NewGuid()}{Path.GetFileName(file.FileName)}";

            ///3. Get File Path
            var filePath = Path.Combine(folderPath, fileName);

            ///4. Save File as Streams [Stream: Data Per Time]
            using var fs = new FileStream(filePath, FileMode.Create);

            file.CopyTo(fs);

            return fileName;
        }

        public static void DeleteFile(string fileName)
        {
            string imageName = Path.GetFileName(fileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/products", imageName);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
