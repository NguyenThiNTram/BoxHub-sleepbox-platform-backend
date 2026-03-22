using BoxHub.Application.DTOs.Requests.Hosts;
using FluentValidation;

namespace BoxHub.Application.Validators.Hosts;

public sealed class ModeratorReviewHostDraftRequestValidator : AbstractValidator<ModeratorReviewHostDraftRequest>
{
    public ModeratorReviewHostDraftRequestValidator()
    {
        RuleFor(x => x.Action).NotEmpty()
            .Must(a => a.Equals("approve", StringComparison.OrdinalIgnoreCase) ||
                       a.Equals("reject", StringComparison.OrdinalIgnoreCase))
            .WithMessage("action phải là approve hoặc reject.");

        When(x => x.Action.Equals("reject", StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(x => x.RejectReason).NotEmpty().MaximumLength(2000);
        });
    }
}
