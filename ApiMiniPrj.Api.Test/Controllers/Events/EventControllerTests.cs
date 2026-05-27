
namespace ApiMiniPrj.Api.Test.Controllers.Events;

public class EventControllerTests
{
    [Fact]
    public async Task Get_ShouldReturnOkWithEvents()
    {
        var events = new List<GetEventDto> { new() { Id = 1, Title = "Event" } };
        var service = new RecordingEventService { Events = events };
        var controller = CreateController(service);

        var response = await controller.Get();

        var okResult = Assert.IsType<OkObjectResult>(response);
        Assert.Same(events, okResult.Value);
    }

    [Fact]
    public async Task Post_WithInvalidModel_ShouldReturnBadRequestAndSkipService()
    {
        var service = new RecordingEventService();
        var controller = CreateController(service, createValidator: new StubValidator<EventCreateDto>("Event title is required."));

        var response = await controller.Post(new EventCreateDto());

        var badRequest = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal("Event title is required.", badRequest.Value);
        Assert.False(service.CreateCalled);
    }

    [Fact]
    public async Task AddBannerImage_WithValidModel_ShouldCallService()
    {
        var service = new RecordingEventService();
        var controller = CreateController(service);
        var file = CreateFormFile();

        var response = await controller.AddBannerImage(5, new EventBannerImageUploadDto { BannerImage = file });

        Assert.IsType<OkResult>(response);
        Assert.Equal(5, service.BannerEventId);
        Assert.Same(file, service.BannerImage);
    }

    private static EventController CreateController(
        RecordingEventService service,
        IValidator<EventCreateDto>? createValidator = null,
        IValidator<EventUpdateDto>? updateValidator = null,
        IValidator<EventBannerImageUploadDto>? bannerValidator = null)
    {
        return new EventController(
            service,
            createValidator ?? new StubValidator<EventCreateDto>(),
            updateValidator ?? new StubValidator<EventUpdateDto>(),
            bannerValidator ?? new StubValidator<EventBannerImageUploadDto>());
    }

    private static FormFile CreateFormFile()
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes("image");
        return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "file", "banner.png");
    }

    private sealed class RecordingEventService : IEventService
    {
        public List<GetEventDto> Events { get; set; } = [];
        public bool CreateCalled { get; private set; }
        public int BannerEventId { get; private set; }
        public IFormFile? BannerImage { get; private set; }

        public Task AddBannerImageAsync(int eventId, IFormFile bannerImage)
        {
            BannerEventId = eventId;
            BannerImage = bannerImage;
            return Task.CompletedTask;
        }

        public Task CreateEventAsync(EventCreateDto eventCreateDto)
        {
            CreateCalled = true;
            return Task.CompletedTask;
        }

        public Task DeleteEventAsync(int eventId) => Task.CompletedTask;

        public Task<List<GetEventDto>> GetAllEventsAsync() => Task.FromResult(Events);

        public Task<GetEventDto> GetEventById(int eventId) => Task.FromResult(new GetEventDto { Id = eventId });

        public Task<GetEventDto> GetEventByTitle(string title) => Task.FromResult(new GetEventDto { Title = title });

        public Task UpdateEventAsync(int eventId, EventUpdateDto eventUpdateDto) => Task.CompletedTask;
    }
}
