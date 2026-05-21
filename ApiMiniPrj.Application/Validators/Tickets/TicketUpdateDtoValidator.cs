
namespace ApiMiniPrj.Application.Validators.Tickets
{
    public class TicketUpdateDtoValidator : AbstractValidator<TicketUpdateDto>
    {
        public TicketUpdateDtoValidator()
        {
            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid ticket type.")
                .When(x => x.Type != null);
            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.")
                .When(x => x.Price != null);
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
                .When(x => x.Quantity != null);
            RuleFor(x => x.IsAvaiable)
                .NotNull().WithMessage("IsAvailable must be provided.")
                .When(x => x.IsAvaiable != null);
        }
    }
}
