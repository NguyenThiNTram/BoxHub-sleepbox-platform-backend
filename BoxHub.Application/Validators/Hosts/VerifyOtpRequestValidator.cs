using BoxHub.Application.DTOs.Requests.Hosts;
using FluentValidation;

namespace BoxHub.Application.Validators.Hosts;

public sealed class VerifyOtpRequestValidator : AbstractValidator<VerifyOtpRequest>
{
    public VerifyOtpRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.OtpCode).NotEmpty().Length(6).Matches(@"^\d{6}$")
            .WithMessage("OTP phải gồm 6 chữ số.");
    }
}
