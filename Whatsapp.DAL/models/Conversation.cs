namespace Whatsapp.DAL.models;

public class Conversation
{
    public string Id { get; set; }
    public string? Name { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsGroup { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<ConversationMember>? Members { get; set; }
        = new List<ConversationMember>();

    public ICollection<Message>? Messages { get; set; }
        = new List<Message>();
}