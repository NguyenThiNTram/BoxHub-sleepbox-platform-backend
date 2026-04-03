using BoxHub.Application.DTOs.Requests.Hosts;
using FluentValidation;

namespace BoxHub.Application.Validators.Hosts;

public sealed class UpdateHostPayoutAccountRequestValidator : AbstractValidator<UpdateHostPayoutAccountRequest>
{
    public UpdateHostPayoutAccountRequestValidator()
    {
        RuleFor(x => x.PaymentMethod).NotEmpty().MaximumLength(50);
        RuleFor(x => x.AccountName).MaximumLength(200).When(x => x.AccountName != null);
        RuleFor(x => x.AccountNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.BankName).MaximumLength(150).When(x => x.BankName != null);
    }
}
