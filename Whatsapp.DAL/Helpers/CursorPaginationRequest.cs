namespace Whatsapp.DAL.Helpers;

public class CursorPaginationRequest
{
    public DateTime? Cursor { get; set; }
    public int? Limit { get; set; } = 20;
}