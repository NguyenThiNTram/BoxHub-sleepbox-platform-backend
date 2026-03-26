namespace BoxHub.Application.DTOs.Responses.Hosts;

public sealed class SimpleMessageResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
}
