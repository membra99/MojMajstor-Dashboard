using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class Profession
{
    public int ProfessionId { get; set; }

    public string? ProfessionName { get; set; }

    public virtual ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();

    public virtual ICollection<ProfessionType> ProfessionTypes { get; set; } = new List<ProfessionType>();
}
