using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class AdvertisementProfessionType
{
    public int AdvertisementProfessionTypeId { get; set; }

    public int AdvertisementId { get; set; }

    public int ProfessionTypeId { get; set; }

    public virtual Advertisement Advertisement { get; set; } = null!;

    public virtual ProfessionType ProfessionType { get; set; } = null!;
}
