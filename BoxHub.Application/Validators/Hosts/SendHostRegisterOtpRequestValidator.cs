using BoxHub.Application.DTOs.Requests.Hosts;
using FluentValidation;

namespace BoxHub.Application.Validators.Hosts;

public sealed class SendHostRegisterOtpRequestValidator : AbstractValidator<SendHostRegisterOtpRequest>
{
    public SendHostRegisterOtpRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}

