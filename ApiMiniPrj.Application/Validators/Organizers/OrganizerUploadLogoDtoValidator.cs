
namespace ApiMiniPrj.Application.Validators.Organizers
{
    public class OrganizerUploadLogoDtoValidator : AbstractValidator<OrganizerUploadLogo>
    {
        public OrganizerUploadLogoDtoValidator()
        {
            RuleFor(x => x.Logo)
                .NotNull().WithMessage("Logo image is required.")
                .Must(file => file!.ContentType.StartsWith("image/") == true).WithMessage("Only image files are allowed.");

            RuleFor(x => x.Logo!.Length)
                .Must(length => length > 0).WithMessage("Logo file cannot be empty.");
        }
    }
}
