using BoxHub.Application.DTOs.Requests.Otp;
using FluentValidation;

namespace BoxHub.Application.Validators.Otp;

public sealed class VerifyOtpGenericRequestValidator : AbstractValidator<VerifyOtpGenericRequest>
{
    public VerifyOtpGenericRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.OtpCode).NotEmpty().Length(6);
    }
}

