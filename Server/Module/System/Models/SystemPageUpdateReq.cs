namespace System.Models;
public class SystemPageUpdateReq
{
    public string PageId { get; set; } = null!;

    public string? PageName { get; set; }

    public string? PageUrl { get; set; }

    public string? PageIcon { get; set; }
    public string? ParentId { get; set; }
    public int PageOrder { get; set; } = 0;
}
