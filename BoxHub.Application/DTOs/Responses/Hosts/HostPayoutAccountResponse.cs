namespace BoxHub.Application.DTOs.Responses.Hosts;

public sealed class HostPayoutAccountResponse
{
    /// <summary>null nếu chưa có bản ghi payout.</summary>
    public Guid? AccountId { get; set; }

    public Guid HostId { get; set; }
    public string? PaymentMethod { get; set; }
    public string? AccountName { get; set; }
    public string? AccountNumber { get; set; }
    public string? BankName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
