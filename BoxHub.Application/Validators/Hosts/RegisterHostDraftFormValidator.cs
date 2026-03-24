using BoxHub.Application.DTOs.Requests.Hosts;
using FluentValidation;

namespace BoxHub.Application.Validators.Hosts;

public sealed class RegisterHostDraftFormValidator : AbstractValidator<RegisterHostDraftForm>
{
    public RegisterHostDraftFormValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Phone).MaximumLength(20).When(x => !string.IsNullOrWhiteSpace(x.Phone));
        RuleFor(x => x.FirstName).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.FirstName));
        RuleFor(x => x.LastName).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.LastName));
        RuleFor(x => x.Gender).MaximumLength(20).When(x => !string.IsNullOrWhiteSpace(x.Gender));
        RuleFor(x => x.RepresentativeName).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.RepresentativeName));
        RuleFor(x => x.RepresentativeIdNumber).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.RepresentativeIdNumber));
        RuleFor(x => x.TaxCode).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.TaxCode));
        RuleFor(x => x.BusinessAddress).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.BusinessAddress));
    }
}
