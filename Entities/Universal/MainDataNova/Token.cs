using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class Token
{
    public int TokenId { get; set; }

    public int NumberOfToken { get; set; }

    public double Price { get; set; }

    public string Package { get; set; } = null!;

    public string Description { get; set; } = null!;

    public double OldPrice { get; set; }

    public bool IsRecommended { get; set; }

    public virtual ICollection<DeletedUser> DeletedUsers { get; set; } = new List<DeletedUser>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
