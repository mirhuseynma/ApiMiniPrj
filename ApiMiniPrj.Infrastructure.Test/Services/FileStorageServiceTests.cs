using ApiMiniPrj.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace ApiMiniPrj.Infrastructure.Test.Services;

public class FileStorageServiceTests : IDisposable
{
    private readonly string _rootPath;
    private readonly FileStorageService _service;

    public FileStorageServiceTests()
    {
        _rootPath = Path.Combine(Path.GetTempPath(), "ApiMiniPrjTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_rootPath);

        _service = new FileStorageService(new TestWebHostEnvironment
        {
            ContentRootPath = _rootPath,
            WebRootPath = Path.Combine(_rootPath, "wwwroot")
        });
    }

    [Fact]
    public async Task SaveFileAsync_WithAllowedImage_ShouldCreateFileAndReturnGeneratedName()
    {
        var formFile = CreateFormFile("banner.png", "image-content");

        var fileName = await _service.SaveFileAsync(formFile, "events");

        Assert.EndsWith(".png", fileName);
        Assert.True(File.Exists(Path.Combine(_rootPath, "wwwroot", "uploads", "events", fileName)));
        Assert.NotEqual("banner.png", fileName);
    }

    [Fact]
    public async Task SaveFileAsync_WithUnsupportedExtension_ShouldThrow()
    {
        var formFile = CreateFormFile("banner.txt", "not-image");

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.SaveFileAsync(formFile, "events"));

        Assert.Equal("Only jpg, jpeg, png and webp files are allowed.", exception.Message);
    }

    [Fact]
    public async Task DeleteFile_WithStoredFileName_ShouldRemoveFile()
    {
        var formFile = CreateFormFile("logo.jpg", "image-content");
        var fileName = await _service.SaveFileAsync(formFile, "organizers");

        _service.DeleteFile(fileName, "organizers");

        Assert.False(File.Exists(Path.Combine(_rootPath, "wwwroot", "uploads", "organizers", fileName)));
    }

    public void Dispose()
    {
        if (Directory.Exists(_rootPath))
        {
            Directory.Delete(_rootPath, recursive: true);
        }
    }

    private static FormFile CreateFormFile(string fileName, string content)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "file", fileName);
    }

    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "ApiMiniPrj.Tests";
        public IFileProvider WebRootFileProvider { get; set; } = null!;
        public string WebRootPath { get; set; } = null!;
        public string EnvironmentName { get; set; } = "Testing";
        public string ContentRootPath { get; set; } = null!;
        public IFileProvider ContentRootFileProvider { get; set; } = null!;
    }
}
