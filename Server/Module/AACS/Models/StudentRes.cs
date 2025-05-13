using System.Models;

namespace AACS.Models;
public partial class StudentRes
{
    public string StudentId { get; set; } = null!;
    public string StudentCode { get; set; } = null!;
    public string? ClassId { get; set; }
    public string UserId { get; set; } = null!;
    public virtual UserRes? User { get; set; }
    public virtual ClassRes? Class { get; set; }

}
