
namespace ApiMiniPrj.Application.Validators.Events
{
    public class EventBannerImageUploadValidator : AbstractValidator<EventBannerImageUploadDto>
    {
        public EventBannerImageUploadValidator()
        {
            RuleFor(x => x.BannerImage)
                .NotNull().WithMessage("Image file is required.")
                .Must(file => file!.Length > 0).WithMessage("Image file cannot be empty.")
                .Must(file => file!.ContentType.StartsWith("image/") == true).WithMessage("Only image files are allowed.");
        }
    }
}
