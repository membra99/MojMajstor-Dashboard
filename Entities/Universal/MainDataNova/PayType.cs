using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class PayType
{
    public int PayTypeId { get; set; }

    public string PayTypeName { get; set; } = null!;

    public virtual ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
}
