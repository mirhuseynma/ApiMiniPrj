
namespace ApiMiniPrj.Application.Test.Validators.Tickets;

public class TicketCreateDtoValidatorTests
{
    private readonly TicketCreateDtoValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_WithValidTicket_ShouldPass()
    {
        var dto = new TicketCreateDto
        {
            Type = TicketTypeEnum.VIP,
            Quantity = 10,
            Price = 25,
            IsAvaiable = true,
            EventId = 1
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidQuantityPriceAndType_ShouldFail()
    {
        var dto = new TicketCreateDto
        {
            Type = (TicketTypeEnum)999,
            Quantity = 0,
            Price = 0,
            IsAvaiable = true
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(TicketCreateDto.Quantity));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(TicketCreateDto.Price));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(TicketCreateDto.Type));
    }
}
