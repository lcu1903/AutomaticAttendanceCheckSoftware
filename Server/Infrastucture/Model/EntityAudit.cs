using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public abstract class EntityAudit : Entity
{
    public DateTime? CreateDate { get; set; }
    [MaxLength(128)]
    public string? CreatedUserId { get; set; }
    public DateTime? UpdateDate { get; set; }
    [MaxLength(128)]
    public string? UpdatedUserId { get; set; }
}