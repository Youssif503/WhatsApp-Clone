namespace Whatsapp.DAL.Helpers;

public class ConversationResponse
{
    public string ConversationId { get; set; }
    public string Name { get; set; }
    public string LastMessage { get; set; }
    public DateTime? LastMessageTime { get; set; }
    public string ImageUrl { get; set; }
    public int UnreadCount { get; set; }
}
