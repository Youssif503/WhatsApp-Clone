namespace Whatsapp.BLL.DTOs.Hubs;

public class OnlineUser
{
    public string ConnectionId { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public string? ImageUrl { get; set; }
    public bool isOnline { get; set; }
}