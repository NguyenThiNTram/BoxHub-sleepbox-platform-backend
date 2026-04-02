using BoxHub.Application.DTOs.Requests.Users;
using FluentValidation;

namespace BoxHub.Application.Validators.Users;

public sealed class UpdateUserProfileRequestValidator : AbstractValidator<UpdateUserProfileRequest>
{
    public UpdateUserProfileRequestValidator()
    {
        RuleFor(x => x.Username)
            .MaximumLength(100)
            .When(x => x.Username != null);

        RuleFor(x => x.Phone).MaximumLength(20).When(x => x.Phone != null);
        RuleFor(x => x.FirstName).MaximumLength(100).When(x => x.FirstName != null);
        RuleFor(x => x.LastName).MaximumLength(100).When(x => x.LastName != null);
        RuleFor(x => x.Gender).MaximumLength(50).When(x => x.Gender != null);

        RuleFor(x => x)
            .Must(r =>
                r.Username != null
                || r.Phone != null
                || r.FirstName != null
                || r.LastName != null
                || r.Gender != null
                || r.DateOfBirth.HasValue)
            .WithMessage(
                "Cần ít nhất một trường: username, phone, firstName, lastName, gender hoặc dateOfBirth.");
    }
}
