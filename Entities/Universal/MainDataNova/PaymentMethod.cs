using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public string PaymentMethodName { get; set; } = null!;

    public virtual ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
}
