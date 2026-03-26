using System.Text.Json;
using BoxHub.Application.Models.HostRegistration;

namespace BoxHub.Application.Helpers;

public static class HostRegistrationJson
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public static HostDraftPayloadModel DeserializePayload(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new HostDraftPayloadModel();
        return JsonSerializer.Deserialize<HostDraftPayloadModel>(json, Options) ?? new HostDraftPayloadModel();
    }

    public static string SerializePayload(HostDraftPayloadModel model) =>
        JsonSerializer.Serialize(model, Options);
}
