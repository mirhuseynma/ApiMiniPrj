using ApiMiniPrj.Application.DTOs.Events;
using ApiMiniPrj.Application.Validators.Events;

namespace ApiMiniPrj.Application.Test.Validators.Events;

public class EventCreateDtoValidatorTests
{
    private readonly EventCreateDtoValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_WithFutureDateAndRequiredFields_ShouldPass()
    {
        var dto = new EventCreateDto
        {
            Title = "Developer Meetup",
            Description = "Community event",
            Date = DateTime.Now.AddDays(2),
            Location = "Baku",
            OrganizerId = 1
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WithPastDateAndMissingFields_ShouldFail()
    {
        var dto = new EventCreateDto
        {
            Title = "",
            Description = new string('a', 501),
            Date = DateTime.Now.AddDays(-1),
            Location = ""
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(EventCreateDto.Title));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(EventCreateDto.Description));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(EventCreateDto.Date));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(EventCreateDto.Location));
    }
}
