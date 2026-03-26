using BoxHub.Application.DTOs.Requests.Hosts;
using FluentValidation;

namespace BoxHub.Application.Validators.Hosts;

public sealed class RegisterHostDraftFormValidator : AbstractValidator<RegisterHostDraftForm>
{
    public RegisterHostDraftFormValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.Username).MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Username));

        RuleFor(x => x.Phone).MaximumLength(20)
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.FirstName).MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.FirstName));

        RuleFor(x => x.LastName).MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.LastName));

        // host_profile
        RuleFor(x => x.RepresentativeIdName).MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.RepresentativeIdName));

        RuleFor(x => x.RepresentativeIdNumber).MaximumLength(10)
            .When(x => !string.IsNullOrWhiteSpace(x.RepresentativeIdNumber));

        RuleFor(x => x.TaxCode).MaximumLength(10)
            .When(x => !string.IsNullOrWhiteSpace(x.TaxCode));

        RuleFor(x => x.BusinessName).MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.BusinessName));

        // Address
        RuleFor(x => x.AddressDistrict).MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.AddressDistrict));

        RuleFor(x => x.AddressWard).MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.AddressWard));

        RuleFor(x => x.AddressDetail).MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.AddressDetail));

        // Brand
        RuleFor(x => x.BrandName).MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.BrandName));

        // Payout account
        RuleFor(x => x.BankName).MaximumLength(150)
            .When(x => !string.IsNullOrWhiteSpace(x.BankName));

        RuleFor(x => x.BankBranch).MaximumLength(150)
            .When(x => !string.IsNullOrWhiteSpace(x.BankBranch));

        RuleFor(x => x.AccountNumber).MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.AccountNumber));

        RuleFor(x => x.AccountName).MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.AccountName));

        RuleFor(x => x.PaymentMethod).MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.PaymentMethod));
    }
}
