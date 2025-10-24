using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class AdvertisementType
{
    public int AdvertisementTypeId { get; set; }

    public string? AdvertisementTypeName { get; set; }

    public virtual ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
}
