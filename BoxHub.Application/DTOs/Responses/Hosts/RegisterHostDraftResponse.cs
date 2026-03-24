using System.Text.Json.Serialization;

namespace BoxHub.Application.DTOs.Responses.Hosts;

public sealed class RegisterHostDraftResponse
{
    [JsonPropertyName("draft_id")]
    public Guid DraftId { get; set; }

    public string Message { get; set; } = "";
}
