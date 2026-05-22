namespace ApiMiniPrj.Application.Validators.Auth
{
    public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
    {
        public ForgotPasswordDtoValidator()
        {
            RuleFor(f => f.Email).NotEmpty().WithMessage("Email is required");
        }
    }
}
