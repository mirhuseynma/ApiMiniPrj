using ApiMiniPrj.Application.DTOs.Events;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMiniPrj.Application.Validators.Events
{
    public class EventUpdateDtoValidator : AbstractValidator<EventUpdateDto>
    {
        public EventUpdateDtoValidator()
        {
            RuleFor(x => x.Title)
                .MinimumLength(10).WithMessage("Event title must be at least 10 characters long.")
                .MaximumLength(100).WithMessage("Event title cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Title));
            RuleFor(x => x.Description)
                .MinimumLength(20).WithMessage("Event description must be at least 20 characters long.")
                .MaximumLength(500).WithMessage("Event description cannot exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
            RuleFor(x => x.Date)
                .GreaterThan(DateTime.Now).WithMessage("Event date must be in the future.")
                .When(x => x.Date.HasValue);
            RuleFor(x => x.Location)
                .MinimumLength(5).WithMessage("Event location must be at least 5 characters long.")
                .MaximumLength(200).WithMessage("Event location cannot exceed 200 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Location));
        }
    }
}
