using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class TokenUser
{
    public int TokenUserId { get; set; }

    public int TokensSummary { get; set; }

    public int UsersId { get; set; }

    public bool IsActive { get; set; }

    public virtual User Users { get; set; } = null!;
}
