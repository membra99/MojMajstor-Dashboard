using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class Invitation
{
    public int InvitationId { get; set; }

    public int OriginUserId { get; set; }

    public int InvitedUserId { get; set; }

    public virtual User InvitedUser { get; set; } = null!;

    public virtual User OriginUser { get; set; } = null!;
}
