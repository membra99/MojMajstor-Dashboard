using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class ProfessionType
{
    public int ProfessionTypeId { get; set; }

    public int ProfessionId { get; set; }

    public string? ProfessionTypeName { get; set; }

    public virtual ICollection<AdvertisementProfessionType> AdvertisementProfessionTypes { get; set; } = new List<AdvertisementProfessionType>();

    public virtual Profession Profession { get; set; } = null!;
}
