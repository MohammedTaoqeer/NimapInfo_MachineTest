using OnlineStore.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace OnlineStore.Repository
{
    public class FileUploadService : IFileUploadService
    {

        public async Task<string> SaveProductImageAsync(HttpPostedFileBase selectedFile)
        {
            if (selectedFile == null)
            {
                throw new ArgumentNullException(nameof(selectedFile), "No file uploaded.");
            }

            string directoryPath = HttpContext.Current.Server.MapPath("~/Uploads/");

            EnsureDirectoryExists(directoryPath);

            string filePath = Path.Combine(directoryPath, selectedFile.FileName);
            await Task.Run(() => selectedFile.SaveAs(filePath));

            return selectedFile.FileName;
        }
        public void EnsureDirectoryExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}