namespace Whatsapp.BLL.DTOs.Messages;

public class CreateMessageDto
{
    public string ConversationId { get; set; }
    public string Content { get; set; }
    public string SenderId { get; set; }
    public DateTime? CreatedAt { get; set; }
}