using Infrastructure.Models;

namespace DataAccess.Models;
public class SystemPage : EntityAudit
{
    public string PageId { get; set; } = null!;

    public string? PageName { get; set; }

    public string? PageUrl { get; set; }

    public string? PageIcon { get; set; }

    public string? ParentId { get; set; }
}
