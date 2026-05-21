
namespace ApiMiniPrj.Application.Validators.Tickets
{
    public class TicketCreateDtoValidator : AbstractValidator<TicketCreateDto>
    {
        public TicketCreateDtoValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");
            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid ticket type.");
            RuleFor(x => x.IsAvaiable)
                .NotNull().WithMessage("Availability status is required.");

        }
    }
}
