namespace ApiMiniPrj.Api.Test.Controllers.Tickets;

public class TicketControllerTests
{
    [Fact]
    public async Task Get_ShouldReturnOkWithTickets()
    {
        var tickets = new[]
        {
            new GetTicketDto { Id = 1, Type = TicketTypeEnum.Standard, Quantity = 5, Price = 10, IsAvaiable = true }
        };
        var service = new RecordingTicketService { Tickets = tickets };
        var controller = CreateController(service);

        var response = await controller.Get();

        var okResult = Assert.IsType<OkObjectResult>(response);
        Assert.Same(tickets, okResult.Value);
    }

    [Fact]
    public async Task Post_WithInvalidModel_ShouldReturnBadRequestAndSkipService()
    {
        var service = new RecordingTicketService();
        var controller = CreateController(
            service,
            createValidator: new StubValidator<TicketCreateDto>("Quantity must be greater than 0."));

        var response = await controller.Post(new TicketCreateDto());

        var badRequest = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal("Quantity must be greater than 0.", badRequest.Value);
        Assert.False(service.CreateCalled);
    }

    [Fact]
    public async Task Post_WithValidModel_ShouldCreateTicketAndReturnOk()
    {
        var service = new RecordingTicketService();
        var controller = CreateController(service);
        var dto = new TicketCreateDto
        {
            EventId = 1,
            Type = TicketTypeEnum.VIP,
            Quantity = 2,
            Price = 100,
            IsAvaiable = true
        };

        var response = await controller.Post(dto);

        Assert.IsType<OkResult>(response);
        Assert.True(service.CreateCalled);
        Assert.Same(dto, service.CreatedTicket);
    }

    private static TicketController CreateController(
        RecordingTicketService service,
        IValidator<TicketCreateDto>? createValidator = null,
        IValidator<TicketUpdateDto>? updateValidator = null)
    {
        return new TicketController(
            service,
            appDbContext: null!,
            createValidator ?? new StubValidator<TicketCreateDto>(),
            updateValidator ?? new StubValidator<TicketUpdateDto>());
    }

    private sealed class RecordingTicketService : ITicketService
    {
        public IEnumerable<GetTicketDto> Tickets { get; set; } = [];
        public bool CreateCalled { get; private set; }
        public TicketCreateDto? CreatedTicket { get; private set; }

        public Task CreateTicketAsync(TicketCreateDto createTicketDto)
        {
            CreateCalled = true;
            CreatedTicket = createTicketDto;
            return Task.CompletedTask;
        }

        public Task DeleteTicketAsync(int ticketId)
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<GetTicketDto>> GetAllTicketsAsync()
        {
            return Task.FromResult(Tickets);
        }

        public Task<GetTicketDto> GetTicketByIdAsync(int ticketId)
        {
            return Task.FromResult(new GetTicketDto { Id = ticketId });
        }

        public Task UpdateTicketAsync(int ticketId, TicketUpdateDto updateTicketDto)
        {
            return Task.CompletedTask;
        }
    }
}
