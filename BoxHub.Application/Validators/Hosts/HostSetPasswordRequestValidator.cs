using BoxHub.Application.DTOs.Requests.Hosts;
using FluentValidation;

namespace BoxHub.Application.Validators.Hosts;

public sealed class HostSetPasswordRequestValidator : AbstractValidator<HostSetPasswordRequest>
{
    public HostSetPasswordRequestValidator()
    {
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8)
            .WithMessage("Mật khẩu tối thiểu 8 ký tự.");
        RuleFor(x => x.ConfirmPassword).NotEmpty().Equal(x => x.NewPassword)
            .WithMessage("Mật khẩu xác nhận phải khớp.");
    }
}
