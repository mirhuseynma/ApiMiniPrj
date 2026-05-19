using ApiMiniPrj.Application.DTOs.Organizers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .MaximumLength(20).WithMessage("Organizer phone number cannot exceed 20 characters.");
            
        }
    }
}
