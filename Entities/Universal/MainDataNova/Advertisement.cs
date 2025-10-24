using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class Advertisement
{
    public int AdvertisementId { get; set; }

    public int AdvertisementTypeId { get; set; }

    public int ProfessionId { get; set; }

    public int? MediaId { get; set; }

    public string? Description { get; set; }

    public DateTime PostedDate { get; set; }

    public int? PayTypeId { get; set; }

    public int? Price { get; set; }

    public int UsersId { get; set; }

    public bool IsClient { get; set; }

    public bool IsActive { get; set; }

    public int? PaymentMethodId { get; set; }

    public int ClickCount { get; set; }

    public virtual ICollection<AdvertisementOpstine> AdvertisementOpstines { get; set; } = new List<AdvertisementOpstine>();

    public virtual ICollection<AdvertisementProfessionType> AdvertisementProfessionTypes { get; set; } = new List<AdvertisementProfessionType>();

    public virtual AdvertisementType AdvertisementType { get; set; } = null!;

    public virtual ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();

    public virtual ICollection<MakeDeal> MakeDeals { get; set; } = new List<MakeDeal>();

    public virtual PayType? PayType { get; set; }

    public virtual PaymentMethod? PaymentMethod { get; set; }

    public virtual Profession Profession { get; set; } = null!;

    public virtual User Users { get; set; } = null!;
}
