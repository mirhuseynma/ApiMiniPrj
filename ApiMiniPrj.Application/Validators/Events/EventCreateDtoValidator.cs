using ApiMiniPrj.Application.DTOs.Events;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMiniPrj.Application.Validators.Events
{
    public class EventCreateDtoValidator : AbstractValidator<EventCreateDto>
    {
        public EventCreateDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Event title is required.")
                .MaximumLength(100).WithMessage("Event title cannot exceed 100 characters.");
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Event description cannot exceed 500 characters.");
            RuleFor(x => x.Date)
                .GreaterThan(DateTime.Now).WithMessage("Event date must be in the future.");
            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Event location is required.")
                .MaximumLength(200).WithMessage("Event location cannot exceed 200 characters.");
            
        }
    }
}
