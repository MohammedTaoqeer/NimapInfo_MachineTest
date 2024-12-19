using System.Threading.Tasks;
using System.Web;

namespace OnlineStore.Services
{
    public interface IFileUploadService
    {
        Task<string> SaveProductImageAsync(HttpPostedFileBase selectedFile);
        void EnsureDirectoryExists(string directoryPath);
    }
}
