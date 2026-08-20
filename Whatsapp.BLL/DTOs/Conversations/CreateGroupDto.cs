namespace Whatsapp.BLL.DTOs;

public class CreateGroupDto
{
    public string MemberId { get; set; }
    public string GroupName { get; set; }
    public List<string> Members { get; set; } = new List<string>();
}