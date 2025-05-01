namespace System.Models;
public class SystemPositionUpdateReq
{
    public string PositionId { get; set; } = null!;

    public string? PositionName { get; set; }

    public string? PositionParentId { get; set; }
    public string? Description { get; set; }
}