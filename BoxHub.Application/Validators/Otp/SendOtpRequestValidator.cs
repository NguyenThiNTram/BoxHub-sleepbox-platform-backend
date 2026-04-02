using BoxHub.Application.DTOs.Requests.Otp;
using FluentValidation;

namespace BoxHub.Application.Validators.Otp;

public sealed class SendOtpRequestValidator : AbstractValidator<SendOtpRequest>
{
    public SendOtpRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

