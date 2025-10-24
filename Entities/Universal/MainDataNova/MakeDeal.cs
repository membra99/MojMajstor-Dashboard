using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class MakeDeal
{
    public int MakeDealId { get; set; }

    public int AdvertisementId { get; set; }

    public int? FirstUserId { get; set; }

    public int? SecondUserId { get; set; }

    public DateTime? AgreementReachedTime { get; set; }

    public bool FirstUserAccept { get; set; }

    public bool FirstUserGiveRating { get; set; }

    public bool SecondUserAccept { get; set; }

    public bool SecondUserGiveRating { get; set; }

    public virtual Advertisement Advertisement { get; set; } = null!;

    public virtual User? FirstUser { get; set; }

    public virtual User? SecondUser { get; set; }
}
