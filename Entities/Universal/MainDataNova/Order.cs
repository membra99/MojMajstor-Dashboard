using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public int UsersId { get; set; }

    public int TokenId { get; set; }

    public virtual Token Token { get; set; } = null!;

    public virtual User Users { get; set; } = null!;
}
