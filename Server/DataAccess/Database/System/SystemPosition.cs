using Infrastructure.Models;

namespace DataAccess.Models;
public class SystemPosition : EntityAudit
{
    public string PositionId { get; set; } = null!;

    public string? PositionName { get; set; }

    public string? PositionParentId { get; set; }
    public string? Description { get; set; }
    public virtual SystemPosition? PositionParent { get; set; }
}
