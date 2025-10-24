using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class AdvertisementOpstine
{
    public int AdvertisementOpstineId { get; set; }

    public int AdvertisementId { get; set; }

    public int OpstineId { get; set; }

    public virtual Advertisement Advertisement { get; set; } = null!;

    public virtual Opstine Opstine { get; set; } = null!;
}
