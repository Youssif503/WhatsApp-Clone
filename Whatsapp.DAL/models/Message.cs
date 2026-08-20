namespace Whatsapp.DAL.models;

public class Message
{
    public string Id { get; set; }
    public string? Content { get; set; }
    public string ConversationId { get; set; }
    public DateTime? SentAt { get; set; } =  DateTime.UtcNow;
    public bool IsRead { get; set; }
    public string? SenderId { get; set; }
    public User? Sender { get; set; }
    public Conversation Conversation { get; set; }
}