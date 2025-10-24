using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class Bookmark
{
    public int BookmarkId { get; set; }

    public int UsersId { get; set; }

    public int AdvertisementId { get; set; }

    public virtual Advertisement Advertisement { get; set; } = null!;

    public virtual User Users { get; set; } = null!;
}
