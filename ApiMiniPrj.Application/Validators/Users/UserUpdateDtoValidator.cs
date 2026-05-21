using ApiMiniPrj.Application.DTOs.Users;

namespace ApiMiniPrj.Application.Validators.Users
{
    public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateDtoValidator()
        {
            RuleFor(x => x.FullName)
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.")
                .When(x => x.FullName != null);
            RuleFor(x => x.UserName)
                .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.")
                .When(x => x.UserName != null);
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format.")
                .When(x => x.Email != null);
            RuleFor(x => x.Password)
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .When(x => x.Password != null);
        }
    }
}
