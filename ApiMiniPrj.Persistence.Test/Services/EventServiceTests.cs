
namespace ApiMiniPrj.Persistence.Test.Services;

public class EventServiceTests
{
    [Fact]
    public async Task CreateEventAsync_WhenOrganizerExists_ShouldPersistEventAndBanner()
    {
        await using var context = ServiceTestFactory.CreateContext();
        context.Organizers.Add(new Organizer { Id = 1, FullName = "Organizer", Email = "org@example.com", PhoneNumber = "123" });
        await context.SaveChangesAsync();
        var storage = new ServiceTestFactory.FakeFileStorageService { SavedFileName = "banner.png" };
        var service = new EventService(context, storage, ServiceTestFactory.CreateMapper());

        await service.CreateEventAsync(new EventCreateDto
        {
            OrganizerId = 1,
            Title = "Event",
            Description = "Description",
            Date = DateTime.UtcNow.AddDays(1),
            Location = "Baku",
            BannerImage = ServiceTestFactory.CreateFormFile()
        });

        var eventEntity = await context.Events.SingleAsync();
        Assert.Equal("Event", eventEntity.Title);
        Assert.Equal(1, eventEntity.OrganizerId);
        Assert.Equal("banner.png", eventEntity.BannerImageUrl);
        Assert.Equal("events", Assert.Single(storage.SaveFolders));
    }

    [Fact]
    public async Task CreateEventAsync_WhenOrganizerDoesNotExist_ShouldThrow()
    {
        await using var context = ServiceTestFactory.CreateContext();
        var service = new EventService(context, new ServiceTestFactory.FakeFileStorageService(), ServiceTestFactory.CreateMapper());

        var exception = await Assert.ThrowsAsync<Exception>(() => service.CreateEventAsync(new EventCreateDto { OrganizerId = 9 }));

        Assert.Equal("organizer not found", exception.Message);
    }

    [Fact]
    public async Task UpdateEventAsync_WithNewBanner_ShouldDeleteOldBannerAndSaveNewOne()
    {
        await using var context = ServiceTestFactory.CreateContext();
        context.Organizers.Add(new Organizer { Id = 1, FullName = "Organizer", Email = "org@example.com", PhoneNumber = "123" });
        context.Events.Add(new Event
        {
            Id = 1,
            OrganizerId = 1,
            Title = "Old",
            Description = "Old description",
            Date = DateTime.UtcNow.AddDays(1),
            Location = "Old location",
            BannerImageUrl = "old.png"
        });
        await context.SaveChangesAsync();
        var storage = new ServiceTestFactory.FakeFileStorageService { SavedFileName = "new.png" };
        var service = new EventService(context, storage, ServiceTestFactory.CreateMapper());

        await service.UpdateEventAsync(1, new EventUpdateDto
        {
            Title = "New",
            BannerImage = ServiceTestFactory.CreateFormFile("new.png")
        });

        var eventEntity = await context.Events.SingleAsync();
        Assert.Equal("New", eventEntity.Title);
        Assert.Equal("Old description", eventEntity.Description);
        Assert.Equal("new.png", eventEntity.BannerImageUrl);
        Assert.Contains(storage.DeletedFiles, file => file.FileName == "old.png" && file.FolderName == "events");
    }

    [Fact]
    public async Task DeleteEventAsync_ShouldSoftDeleteEventAndDeleteBanner()
    {
        await using var context = ServiceTestFactory.CreateContext();
        context.Organizers.Add(new Organizer { Id = 1, FullName = "Organizer", Email = "org@example.com", PhoneNumber = "123" });
        context.Events.Add(new Event { Id = 1, OrganizerId = 1, Title = "Event", Description = "Description", Location = "Baku", BannerImageUrl = "banner.png" });
        await context.SaveChangesAsync();
        var storage = new ServiceTestFactory.FakeFileStorageService();
        var service = new EventService(context, storage, ServiceTestFactory.CreateMapper());

        await service.DeleteEventAsync(1);

        var eventEntity = await context.Events.IgnoreQueryFilters().SingleAsync();
        Assert.True(eventEntity.IsDeleted);
        Assert.Contains(storage.DeletedFiles, file => file.FileName == "banner.png" && file.FolderName == "events");
    }

    [Fact]
    public async Task GetAllEventsAsync_ShouldExcludeDeletedEventsAndDeletedTickets()
    {
        await using var context = ServiceTestFactory.CreateContext();
        context.Organizers.Add(new Organizer { Id = 1, FullName = "Organizer", Email = "org@example.com", PhoneNumber = "123" });
        context.Events.Add(new Event
        {
            Id = 1,
            OrganizerId = 1,
            Title = "Visible",
            Description = "Description",
            Location = "Baku",
            Tickets =
            [
                new Ticket { Id = 1, Quantity = 5, Price = 10 },
                new Ticket { Id = 2, Quantity = 1, Price = 5, IsDeleted = true }
            ]
        });
        context.Events.Add(new Event { Id = 2, OrganizerId = 1, Title = "Deleted", Description = "Description", Location = "Baku", IsDeleted = true });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var service = new EventService(context, new ServiceTestFactory.FakeFileStorageService(), ServiceTestFactory.CreateMapper());

        var result = await service.GetAllEventsAsync();

        var eventDto = Assert.Single(result);
        Assert.Equal("Visible", eventDto.Title);
        Assert.Single(eventDto.Tickets);
    }

    [Fact]
    public async Task AddBannerImageAsync_ShouldReplaceExistingBanner()
    {
        await using var context = ServiceTestFactory.CreateContext();
        context.Events.Add(new Event { Id = 1, Title = "Event", Description = "Description", Location = "Baku", BannerImageUrl = "old.png" });
        await context.SaveChangesAsync();
        var storage = new ServiceTestFactory.FakeFileStorageService { SavedFileName = "new.png" };
        var service = new EventService(context, storage, ServiceTestFactory.CreateMapper());

        await service.AddBannerImageAsync(1, ServiceTestFactory.CreateFormFile("new.png"));

        var eventEntity = await context.Events.SingleAsync();
        Assert.Equal("new.png", eventEntity.BannerImageUrl);
        Assert.Contains(storage.DeletedFiles, file => file.FileName == "old.png" && file.FolderName == "events");
    }
}
