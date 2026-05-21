
namespace ApiMiniPrj.Application.Validators.Auth
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.").MinimumLength(3).WithMessage("Username must be at least 3 characters long.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress().WithMessage("Invalid email address.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.").MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
            RuleFor(x => x.RePassword).NotEmpty().WithMessage("Please confirm your password.").Equal(x => x.Password).WithMessage("Passwords do not match.");
            RuleFor(x => x.AcceptTerms).NotEmpty().WithMessage("You must accept the terms and conditions.");
        }
    }
}
