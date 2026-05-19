using ApiMiniPrj.Application.DTOs.Organizers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMiniPrj.Application.Validators.Organizers
{
    public class OrganizerUpdateDtoValidator : AbstractValidator<OrganizerUpdateDto>
    {
        public OrganizerUpdateDtoValidator()
        {
            RuleFor(x => x.FullName)
                .MinimumLength(5).WithMessage("Organizer fullname must be at least 5 characters long.")
                .MaximumLength(100).WithMessage("Organizer fullname cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.FullName));
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(100).WithMessage("Organizer email cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));
            RuleFor(x => x.PhoneNumber)
                .MinimumLength(7).WithMessage("Organizer phone number must be at least 7 characters long.")
                .MaximumLength(20).WithMessage("Organizer phone number cannot exceed 20 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
        }
    }
}
