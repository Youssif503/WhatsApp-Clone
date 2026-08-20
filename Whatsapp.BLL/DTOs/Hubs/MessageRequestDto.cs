namespace Whatsapp.BLL.DTOs.Hubs;

public class MessageRequestDto
{
    public string? SenderId { get; set; }
    public string? ConversationId { get; set; }
    public string? Content { get; set; }
    public bool IsRead { get; set; }
    public DateTime? CreatedDate { get; set; }
}