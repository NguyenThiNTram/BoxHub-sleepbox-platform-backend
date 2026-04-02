using BoxHub.Application.Constants;
using BoxHub.Application.DTOs.Requests.Otp;
using BoxHub.Application.DTOs.Responses.Hosts;
using BoxHub.Application.DTOs.Responses.Otp;
using BoxHub.Application.Interfaces;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums;
using BoxHub.Infrastructure.Domain.Entities;
using BoxHub.Shared.Errors;
using BoxHub.Shared.Helpers;
using BoxHub.Shared.Results;
using System.Net.Mail;

namespace BoxHub.Application.Services;

public sealed class OtpService : IOtpService
{
    private readonly IHostRegistrationRepository _repo;
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly IEmailService _email;
    private readonly IJwtService _jwt;

    public OtpService(
        IHostRegistrationRepository repo,
        IUserRepository users,
        IUnitOfWork uow,
        IEmailService email,
        IJwtService jwt)
    {
        _repo = repo;
        _users = users;
        _uow = uow;
        _email = email;
        _jwt = jwt;
    }

    public async Task<Result<SendOtpResponse>> SendAsync(SendOtpRequest request, CancellationToken ct)
    {
        var email = NormalizeEmail(request.Email);
        if (string.IsNullOrWhiteSpace(email))
            return Result<SendOtpResponse>.Failure(ErrorCodes.ValidationFailed, "Email không hợp lệ.", 400);

        try
        {
            _ = new MailAddress(email);
        }
        catch
        {
            return Result<SendOtpResponse>.Failure(ErrorCodes.ValidationFailed, "Email không hợp lệ.", 400);
        }

        if (request.Purpose == OTPPurpose.HOST_REGISTER)
        {
            var existingUser = await _users.GetByEmailAsync(email, ct);
            if (existingUser != null && existingUser.role == UserRole.Host)
            {
                return Result<SendOtpResponse>.Failure(
                    ErrorCodes.EmailExists,
                    "Email đã được đăng ký làm Host.",
                    409);
            }
        }

        var now = DateTime.UtcNow;

        var latest = await _repo.GetLatestOtpForEmailAsync(email, request.Purpose, ct);
        if (latest != null && (now - latest.created_at).TotalSeconds < 60)
        {
            return Result<SendOtpResponse>.Failure(
                ErrorCodes.OtpRateLimited,
                "Vui lòng đợi ít nhất 60 giây trước khi gửi OTP.",
                429);
        }

        var count = await _repo.CountOtpsCreatedSinceAsync(email, request.Purpose, now.AddMinutes(-10), ct);
        if (count >= 5)
        {
            return Result<SendOtpResponse>.Failure(
                ErrorCodes.OtpRateLimited,
                "Đã vượt quá 5 lần gửi OTP trong 10 phút.",
                429);
        }

        var otp = new email_otp
        {
            otp_id = Guid.NewGuid(),
            email = email,
            otp_code = GenerateOtpCode(),
            purpose = request.Purpose,
            expire_at = now.AddMinutes(10),
            is_used = false,
            attempt_count = 0,
            created_at = now
        };

        await _repo.AddOtpAsync(otp, ct);
        await _uow.SaveChangesAsync(ct);

        var sent = await _email.SendEmailAsync(new MailData
        {
            EmailToId = email,
            EmailToName = email,
            EmailSubject = "BoxHub — Mã OTP",
            EmailBody = $"<p>Mã OTP của bạn: <strong>{otp.otp_code}</strong> (hiệu lực 10 phút).</p>"
        });

        if (!sent)
            return Result<SendOtpResponse>.Failure(ErrorCodes.EmailSendFailed, "Không gửi được email OTP. Vui lòng thử lại sau.", 500);

        return Result<SendOtpResponse>.Success(new SendOtpResponse
        {
            Success = true,
            Message = "Đã gửi OTP qua email."
        });
    }

    public async Task<Result<VerifyOtpResponse>> VerifyAsync(VerifyOtpGenericRequest request, CancellationToken ct)
    {
        var email = NormalizeEmail(request.Email);
        var code = (request.OtpCode ?? "").Trim();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
            return Result<VerifyOtpResponse>.Failure(ErrorCodes.ValidationFailed, "Email/OTP không hợp lệ.", 400);

        var now = DateTime.UtcNow;
        var otpMatch = await _repo.FindValidOtpAsync(email, code, request.Purpose, now, ct);
        if (otpMatch == null)
        {
            var latestOtp = await _repo.GetLatestOtpForEmailAsync(email, request.Purpose, ct);
            if (latestOtp != null && !latestOtp.is_used && latestOtp.expire_at > now)
            {
                latestOtp.attempt_count += 1;
                await _repo.UpdateOtpAsync(latestOtp, ct);
                await _uow.SaveChangesAsync(ct);
            }

            return Result<VerifyOtpResponse>.Failure(ErrorCodes.OtpInvalid, "Mã OTP không đúng hoặc đã hết hạn.", 400);
        }

        var trackedOtp = await _repo.GetOtpByIdAsync(otpMatch.otp_id, track: true, ct);
        if (trackedOtp == null)
            return Result<VerifyOtpResponse>.Failure(ErrorCodes.OtpInvalid, "OTP không hợp lệ.", 400);

        trackedOtp.is_used = true;
        await _repo.UpdateOtpAsync(trackedOtp, ct);

        await _uow.SaveChangesAsync(ct);

        string? token = null;
        if (request.Purpose == OTPPurpose.HOST_REGISTER)
        {
            token = _jwt.CreateHostRegistrationToken(
                Guid.NewGuid(),
                email,
                HostRegistrationTokenUses.EmailVerified,
                TimeSpan.FromDays(14));
        }

        return Result<VerifyOtpResponse>.Success(new VerifyOtpResponse
        {
            Success = true,
            Message = "Xác thực OTP thành công.",
            Token = token
        });
    }

    public async Task<Result<SimpleMessageResponse>> ResendAsync(ResendOtpGenericRequest request, CancellationToken ct)
    {
        var email = NormalizeEmail(request.Email);
        if (string.IsNullOrWhiteSpace(email))
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.ValidationFailed, "Email không hợp lệ.", 400);

        var now = DateTime.UtcNow;
        var latest = await _repo.GetLatestOtpForEmailAsync(email, request.Purpose, ct);
        if (latest != null && (now - latest.created_at).TotalSeconds < 60)
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.OtpRateLimited, "Vui lòng đợi ít nhất 60 giây trước khi gửi lại OTP.", 429);

        // Nếu OTP gần nhất chưa dùng và chưa hết hạn thì gửi lại đúng mã.
        if (latest != null && !latest.is_used && latest.expire_at > now)
        {
            var sent = await _email.SendEmailAsync(new MailData
            {
                EmailToId = email,
                EmailToName = email,
                EmailSubject = "BoxHub — Mã OTP",
                EmailBody = $"<p>Mã OTP của bạn: <strong>{latest.otp_code}</strong> (hiệu lực 10 phút).</p>"
            });

            if (!sent)
                return Result<SimpleMessageResponse>.Failure(ErrorCodes.EmailSendFailed, "Không gửi được email OTP. Vui lòng thử lại sau.", 500);

            return Result<SimpleMessageResponse>.Success(new SimpleMessageResponse
            {
                Success = true,
                Message = "Đã gửi lại mã OTP qua email."
            });
        }

        var count = await _repo.CountOtpsCreatedSinceAsync(email, request.Purpose, now.AddMinutes(-10), ct);
        if (count >= 5)
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.OtpRateLimited, "Đã vượt quá 5 lần gửi OTP trong 10 phút.", 429);

        var newOtp = new email_otp
        {
            otp_id = Guid.NewGuid(),
            email = email,
            otp_code = GenerateOtpCode(),
            purpose = request.Purpose,
            expire_at = now.AddMinutes(10),
            is_used = false,
            attempt_count = 0,
            created_at = now
        };

        await _repo.AddOtpAsync(newOtp, ct);
        await _uow.SaveChangesAsync(ct);

        var sentNew = await _email.SendEmailAsync(new MailData
        {
            EmailToId = email,
            EmailToName = email,
            EmailSubject = "BoxHub — Mã OTP",
            EmailBody = $"<p>Mã OTP của bạn: <strong>{newOtp.otp_code}</strong> (hiệu lực 10 phút).</p>"
        });

        if (!sentNew)
            return Result<SimpleMessageResponse>.Failure(ErrorCodes.EmailSendFailed, "Không gửi được email OTP. Vui lòng thử lại sau.", 500);

        return Result<SimpleMessageResponse>.Success(new SimpleMessageResponse
        {
            Success = true,
            Message = "Đã gửi mã OTP mới qua email."
        });
    }

    private static string NormalizeEmail(string email) => (email ?? "").Trim().ToLowerInvariant();

    private static string GenerateOtpCode() => Random.Shared.Next(100000, 1_000_000).ToString();
}

