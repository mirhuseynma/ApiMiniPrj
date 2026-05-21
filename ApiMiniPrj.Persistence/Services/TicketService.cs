
namespace ApiMiniPrj.Persistence.Services
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IValidator<TicketCreateDto> _createValidator;
        private readonly IValidator<TicketUpdateDto> _updateValidator;

        public TicketService(AppDbContext context, IMapper mapper, IValidator<TicketCreateDto> createValidator, IValidator<TicketUpdateDto> updateValidator)
        {
            _context = context;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task CreateTicketAsync(TicketCreateDto createTicketDto)
        {
            var validationResult = await _createValidator.ValidateAsync(createTicketDto);

            if (validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                var errorMessage = string.Join("; ", errors);
                throw new Exception(errorMessage);
            }

            var ticket = _mapper.Map<Ticket>(createTicketDto);

            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTicketAsync(int ticketId, TicketUpdateDto updateTicketDto)
        {
            var validationResult = await _updateValidator.ValidateAsync(updateTicketDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                var errorMessage = string.Join("; ", errors);
                throw new Exception(errorMessage);
            }


            var ticket = await GetTicketEntityAsync(ticketId);

            _mapper.Map(updateTicketDto, ticket);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTicketAsync(int ticketId)
        {
            var ticket = await GetTicketEntityAsync(ticketId);

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task<GetTicketDto> GetTicketByIdAsync(int ticketId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Event)
                .FirstOrDefaultAsync(t => t.Id == ticketId && !t.IsDeleted);

            if (ticket is null)
            {
                throw new KeyNotFoundException("Ticket not found.");
            }

            return _mapper.Map<GetTicketDto>(ticket);
        }

        public async Task<IEnumerable<GetTicketDto>> GetAllTicketsAsync()
        {
            var tickets = await _context.Tickets
                .Where(t => !t.IsDeleted)
                .Include(t => t.Event)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GetTicketDto>>(tickets);
        }

        private async Task<Ticket> GetTicketEntityAsync(int ticketId)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId && !t.IsDeleted);

            if (ticket is null)
            {
                throw new KeyNotFoundException("Ticket not found.");
            }

            return ticket;
        }
    }
}
