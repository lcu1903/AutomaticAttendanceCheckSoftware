
using System.Models;

namespace AACS.Models;
public partial class TeacherRes
{
    public string TeacherId { get; set; } = null!;

    public string TeacherCode { get; set; } = null!;
    public string? UserId { get; set; }
    public virtual UserRes? User { get; set; }


}
