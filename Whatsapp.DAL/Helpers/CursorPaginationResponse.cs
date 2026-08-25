namespace Whatsapp.DAL.Helpers;

public class CursorPaginationResponse<T>
{
    public IReadOnlyList<T> data { get; set; } = [];
    public DateTime? NextCursor { get; set; }
    public bool HasNext { get; set; }
}