
namespace ApiMiniPrj.Application.Validators.Organizers
{
    public class OrganizerCreateDtoValidator : AbstractValidator<OrganizerCreateDto>
    {
        public OrganizerCreateDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Organizer fullname is required.")
                .MaximumLength(100).WithMessage("Organizer fullname cannot exceed 100 characters.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Organizer email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(100).WithMessage("Organizer email cannot exceed 100 characters.");
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Organizer phone number is required.")
                .Matches(@"^\+994\d{9}$").WithMessage("Phone number must be in format +994XXXXXXXXX (where X is a digit).")
                .MaximumLength(20).WithMessage("Organizer phone number cannot exceed 20 characters.");
            
        }
    }
}
