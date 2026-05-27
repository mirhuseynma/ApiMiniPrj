
namespace ApiMiniPrj.Api.Test.Controllers.Organizers;

public class OraganizerControllerTests
{
    [Fact]
    public async Task Get_ShouldReturnOkWithOrganizers()
    {
        var organizers = new[] { new GetOrganizerDto { Id = 1, FullName = "Organizer" } };
        var service = new RecordingOrganizerService { Organizers = organizers };
        var controller = CreateController(service);

        var response = await controller.Get();

        var okResult = Assert.IsType<OkObjectResult>(response);
        Assert.Same(organizers, okResult.Value);
    }

    [Fact]
    public async Task Post_WithInvalidModel_ShouldReturnBadRequestAndSkipService()
    {
        var service = new RecordingOrganizerService();
        var controller = CreateController(service, createValidator: new StubValidator<OrganizerCreateDto>("Organizer name is required."));

        var response = await controller.Post(new OrganizerCreateDto());

        var badRequest = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal("Organizer name is required.", badRequest.Value);
        Assert.False(service.CreateCalled);
    }

    [Fact]
    public async Task UploadLogo_WithEmptyLogo_ShouldReturnBadRequestAndSkipService()
    {
        var service = new RecordingOrganizerService();
        var controller = CreateController(service);

        var response = await controller.UploadLogo(1, new OrganizerUploadLogo());

        var badRequest = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal("Logo file is required.", badRequest.Value);
        Assert.False(service.UploadLogoCalled);
    }

    [Fact]
    public async Task UploadLogo_WithValidLogo_ShouldCallService()
    {
        var service = new RecordingOrganizerService();
        var controller = CreateController(service);
        var file = CreateFormFile();

        var response = await controller.UploadLogo(7, new OrganizerUploadLogo { Logo = file });

        Assert.IsType<OkResult>(response);
        Assert.True(service.UploadLogoCalled);
        Assert.Equal(7, service.UploadLogoId);
        Assert.Same(file, service.UploadedLogo);
    }

    private static OraganizerController CreateController(
        RecordingOrganizerService service,
        IValidator<OrganizerCreateDto>? createValidator = null,
        IValidator<OrganizerUpdateDto>? updateValidator = null,
        IValidator<OrganizerUploadLogo>? uploadLogoValidator = null)
    {
        return new OraganizerController(
            service,
            createValidator ?? new StubValidator<OrganizerCreateDto>(),
            updateValidator ?? new StubValidator<OrganizerUpdateDto>(),
            uploadLogoValidator ?? new StubValidator<OrganizerUploadLogo>());
    }

    private static FormFile CreateFormFile()
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes("image");
        return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "file", "logo.png");
    }

    private sealed class RecordingOrganizerService : IOrganizerService
    {
        public IEnumerable<GetOrganizerDto> Organizers { get; set; } = [];
        public bool CreateCalled { get; private set; }
        public bool UploadLogoCalled { get; private set; }
        public int UploadLogoId { get; private set; }
        public IFormFile? UploadedLogo { get; private set; }

        public Task CreateOrganizerAsync(OrganizerCreateDto createOrganizerDto)
        {
            CreateCalled = true;
            return Task.CompletedTask;
        }

        public Task DeleteOrganizerAsync(int id) => Task.CompletedTask;

        public Task<IEnumerable<GetOrganizerDto>> GetAllOrganizersAsync() => Task.FromResult(Organizers);

        public Task<GetOrganizerDto> GetOrganizerByIdAsync(int id) => Task.FromResult(new GetOrganizerDto { Id = id });

        public Task OrganizerUploadLogo(int id, IFormFile logo)
        {
            UploadLogoCalled = true;
            UploadLogoId = id;
            UploadedLogo = logo;
            return Task.CompletedTask;
        }

        public Task UpdateOrganizerAsync(int id, OrganizerUpdateDto updateOrganizerDto) => Task.CompletedTask;
    }
}
