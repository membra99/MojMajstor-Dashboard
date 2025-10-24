using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class Opstine
{
    public int OpstineId { get; set; }

    public string? OpstinaIme { get; set; }

    public virtual ICollection<AdvertisementOpstine> AdvertisementOpstines { get; set; } = new List<AdvertisementOpstine>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
