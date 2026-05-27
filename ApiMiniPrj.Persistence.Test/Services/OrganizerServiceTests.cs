
namespace ApiMiniPrj.Persistence.Test.Services;

public class OrganizerServiceTests
{
    [Fact]
    public async Task CreateOrganizerAsync_WithLogo_ShouldPersistOrganizerAndLogo()
    {
        await using var context = ServiceTestFactory.CreateContext();
        var storage = new ServiceTestFactory.FakeFileStorageService { SavedFileName = "logo.png" };
        var service = new OrganizerService(context, storage, ServiceTestFactory.CreateMapper());

        await service.CreateOrganizerAsync(new OrganizerCreateDto
        {
            FullName = "Organizer",
            Email = "org@example.com",
            PhoneNumber = "123",
            Logo = ServiceTestFactory.CreateFormFile("logo.png")
        });

        var organizer = await context.Organizers.SingleAsync();
        Assert.Equal("Organizer", organizer.FullName);
        Assert.Equal("logo.png", organizer.LogoUrl);
        Assert.Equal("organizers", Assert.Single(storage.SaveFolders));
    }

    [Fact]
    public async Task UpdateOrganizerAsync_WithNewLogo_ShouldDeleteOldLogoAndSaveNewOne()
    {
        await using var context = ServiceTestFactory.CreateContext();
        context.Organizers.Add(new Organizer { Id = 1, FullName = "Old", Email = "old@example.com", PhoneNumber = "111", LogoUrl = "old.png" });
        await context.SaveChangesAsync();
        var storage = new ServiceTestFactory.FakeFileStorageService { SavedFileName = "new.png" };
        var service = new OrganizerService(context, storage, ServiceTestFactory.CreateMapper());

        await service.UpdateOrganizerAsync(1, new OrganizerUpdateDto
        {
            FullName = "New",
            Logo = ServiceTestFactory.CreateFormFile("new.png")
        });

        var organizer = await context.Organizers.SingleAsync();
        Assert.Equal("New", organizer.FullName);
        Assert.Equal("old@example.com", organizer.Email);
        Assert.Equal("new.png", organizer.LogoUrl);
        Assert.Contains(storage.DeletedFiles, file => file.FileName == "old.png" && file.FolderName == "organizers");
    }

    [Fact]
    public async Task DeleteOrganizerAsync_ShouldSoftDeleteOrganizerAndDeleteLogo()
    {
        await using var context = ServiceTestFactory.CreateContext();
        context.Organizers.Add(new Organizer { Id = 1, FullName = "Organizer", Email = "org@example.com", PhoneNumber = "123", LogoUrl = "logo.png" });
        await context.SaveChangesAsync();
        var storage = new ServiceTestFactory.FakeFileStorageService();
        var service = new OrganizerService(context, storage, ServiceTestFactory.CreateMapper());

        await service.DeleteOrganizerAsync(1);

        var organizer = await context.Organizers.IgnoreQueryFilters().SingleAsync();
        Assert.True(organizer.IsDeleted);
        Assert.Contains(storage.DeletedFiles, file => file.FileName == "logo.png" && file.FolderName == "organizers");
    }

    [Fact]
    public async Task GetOrganizerByIdAsync_ShouldExcludeDeletedEvents()
    {
        await using var context = ServiceTestFactory.CreateContext();
        context.Organizers.Add(new Organizer
        {
            Id = 1,
            FullName = "Organizer",
            Email = "org@example.com",
            PhoneNumber = "123",
            Events =
            [
                new Event { Id = 1, Title = "Visible", Description = "Description", Location = "Baku" },
                new Event { Id = 2, Title = "Deleted", Description = "Description", Location = "Baku", IsDeleted = true }
            ]
        });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var service = new OrganizerService(context, new ServiceTestFactory.FakeFileStorageService(), ServiceTestFactory.CreateMapper());

        var result = await service.GetOrganizerByIdAsync(1);

        Assert.Equal("Organizer", result.FullName);
        Assert.Single(result.Events);
    }

    [Fact]
    public async Task OrganizerUploadLogo_WithEmptyLogo_ShouldThrow()
    {
        await using var context = ServiceTestFactory.CreateContext();
        var service = new OrganizerService(context, new ServiceTestFactory.FakeFileStorageService(), ServiceTestFactory.CreateMapper());
        var emptyFile = ServiceTestFactory.CreateFormFile("empty.png", "");

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.OrganizerUploadLogo(1, emptyFile));

        Assert.Contains("Logo is required.", exception.Message);
    }
}
