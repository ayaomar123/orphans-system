using FluentValidation;
using OrphanManagement.Application.DTOs.Events;

namespace OrphanManagement.Application.Validators;

/// <summary>
/// Validator for CreateEventRequest DTO
/// </summary>
public class CreateEventRequestValidator : AbstractValidator<CreateEventRequest>
{
    public CreateEventRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Event title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");
        
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required");
        
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be after or equal to start date");
        
        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required")
            .MaximumLength(200).WithMessage("Location cannot exceed 200 characters");
        
        RuleFor(x => x.EventType)
            .IsInEnum().WithMessage("Invalid event type");
        
        RuleFor(x => x.MaxParticipants)
            .GreaterThan(0).WithMessage("Max participants must be greater than 0")
            .When(x => x.MaxParticipants.HasValue);
        
        RuleFor(x => x.Budget)
            .GreaterThanOrEqualTo(0).WithMessage("Budget cannot be negative")
            .When(x => x.Budget.HasValue);
    }
}
