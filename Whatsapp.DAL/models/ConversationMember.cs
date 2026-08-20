namespace Whatsapp.DAL.models;

public class ConversationMember
{
    public string ConversationId { get; set; }
    public string UserId { get; set; }
    public DateTime JoinedAt { get; set; }
    public Conversation Conversation { get; set; }
    public User User { get; set; }
}