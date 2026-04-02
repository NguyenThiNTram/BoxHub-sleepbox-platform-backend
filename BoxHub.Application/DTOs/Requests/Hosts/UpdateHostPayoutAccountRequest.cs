namespace BoxHub.Application.DTOs.Requests.Hosts;

public sealed class UpdateHostPayoutAccountRequest
{
    public string PaymentMethod { get; set; } = "";
    public string? AccountName { get; set; }
    public string AccountNumber { get; set; } = "";
    public string? BankName { get; set; }
}
