using System;
using System.Collections.Generic;

namespace System.Models;

public partial class RefreshTokenRes
{
    public string Token { get; set; } = null!;

    public string JwtId { get; set; } = null!;

    public DateTime CreationDate { get; set; }

    public DateTime ExpiryDate { get; set; }

    public bool Used { get; set; }

    public bool Invalidated { get; set; }

    public string UserId { get; set; } = null!;
}
