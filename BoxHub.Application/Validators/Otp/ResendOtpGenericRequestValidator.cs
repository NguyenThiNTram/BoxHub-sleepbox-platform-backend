using BoxHub.Application.DTOs.Requests.Otp;
using FluentValidation;

namespace BoxHub.Application.Validators.Otp;

public sealed class ResendOtpGenericRequestValidator : AbstractValidator<ResendOtpGenericRequest>
{
    public ResendOtpGenericRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

