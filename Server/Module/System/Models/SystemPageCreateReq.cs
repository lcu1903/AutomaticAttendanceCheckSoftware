namespace System.Models;
public class SystemPageCreateReq
{
    public string? PageName { get; set; }

    public string? PageUrl { get; set; }

    public string? PageIcon { get; set; }
    public string? ParentId { get; set; }
}