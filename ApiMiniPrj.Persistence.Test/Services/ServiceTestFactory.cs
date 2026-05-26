using ApiMiniPrj.Application.Interfaces.Common;
using ApiMiniPrj.Application.Interfaces.JWT;
using ApiMiniPrj.Application.Mappings;
using ApiMiniPrj.Domain.Models.Users;
using ApiMiniPrj.Persistence.Context;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ApiMiniPrj.Persistence.Test.Services;

internal static class ServiceTestFactory
{
    public static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new AppDbContext(options);
    }

    public static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(config =>
        {
            config.AddProfile<MappingProfile>();
        }, Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);

        return configuration.CreateMapper(serviceType =>
        {
            if (serviceType == typeof(IHttpContextAccessor))
            {
                return new HttpContextAccessor();
            }

            var constructor = serviceType
                .GetConstructors()
                .FirstOrDefault(ctor =>
                {
                    var parameters = ctor.GetParameters();
                    return parameters.Length == 1 && parameters[0].ParameterType == typeof(IHttpContextAccessor);
                });

            if (constructor is not null)
            {
                return constructor.Invoke([new HttpContextAccessor()]);
            }

            return Activator.CreateInstance(serviceType)!;
        });
    }

    public static FormFile CreateFormFile(string fileName = "image.png", string content = "image-content")
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "file", fileName);
    }

    public sealed class FakeFileStorageService : ApiMiniPrj.Application.Interfaces.Common.IFileStorageService
    {
        public List<(string FileName, string FolderName)> DeletedFiles { get; } = [];
        public List<string> SaveFolders { get; } = [];
        public string SavedFileName { get; set; } = "saved-image.png";

        public Task<string> SaveFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default)
        {
            SaveFolders.Add(folderName);
            return Task.FromResult(SavedFileName);
        }

        public void DeleteFile(string? fileName, string folderName)
        {
            if (fileName is not null)
            {
                DeletedFiles.Add((fileName, folderName));
            }
        }
    }

    public sealed class FakeJwtService : IJwtService
    {
        public string Token { get; set; } = "jwt-token";
        public string RefreshToken { get; set; } = "new-refresh-token";

        public Task<string> GenerateRefreshTokenAsync()
        {
            return Task.FromResult(RefreshToken);
        }

        public Task<string> GenerateTokenAsync(AppUser user)
        {
            return Task.FromResult(Token);
        }
    }
}
