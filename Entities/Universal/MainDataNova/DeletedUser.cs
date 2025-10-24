using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class DeletedUser
{
    public int DeletedUserId { get; set; }

    public int TokenId { get; set; }

    public string Email { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public virtual Token Token { get; set; } = null!;
}
