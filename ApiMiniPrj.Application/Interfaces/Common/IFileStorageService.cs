
namespace ApiMiniPrj.Application.Interfaces.Common
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default);
        void DeleteFile(string? fileName, string folderName);
    }
}
