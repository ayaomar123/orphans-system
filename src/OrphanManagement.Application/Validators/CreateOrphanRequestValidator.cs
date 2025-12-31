using FluentValidation;
using OrphanManagement.Application.DTOs.Orphans;

namespace OrphanManagement.Application.Validators;

/// <summary>
/// Validator for CreateOrphanRequest DTO
/// </summary>
public class CreateOrphanRequestValidator : AbstractValidator<CreateOrphanRequest>
{
    public CreateOrphanRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");
        
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");
        
        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender");
        
        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.UtcNow).WithMessage("Date of birth must be in the past")
            .GreaterThan(DateTime.UtcNow.AddYears(-25)).WithMessage("Orphan must be under 25 years old");
        
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City name cannot exceed 100 characters");
        
        RuleFor(x => x.HealthStatus)
            .IsInEnum().WithMessage("Invalid health status");
        
        RuleFor(x => x.EducationStatus)
            .IsInEnum().WithMessage("Invalid education status");
        
        RuleFor(x => x.SponsorshipStatus)
            .IsInEnum().WithMessage("Invalid sponsorship status");
        
        RuleFor(x => x.NationalId)
            .MaximumLength(50).WithMessage("National ID cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.NationalId));
        
        RuleFor(x => x.GuardianPhone)
            .MaximumLength(20).WithMessage("Guardian phone cannot exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.GuardianPhone));
    }
}
