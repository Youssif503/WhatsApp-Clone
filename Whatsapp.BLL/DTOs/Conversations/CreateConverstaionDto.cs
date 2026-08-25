using System.Text.Json.Serialization;

namespace Whatsapp.BLL.DTOs;

public class CreateConverstaionDto
{
    // This value is always supplied by the authenticated user in the controller.
    // It must not be required from, or accepted from, the client request body.
    [JsonIgnore]
    public string? UserMemberId { get; set; }

    public string OtherUserId { get; set; } = string.Empty;
}
