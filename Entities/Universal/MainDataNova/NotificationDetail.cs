using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class NotificationDetail
{
    public int NotificationDetailsId { get; set; }

    public int? UserFrom { get; set; }

    public int UserTo { get; set; }

    public string NotificationFor { get; set; } = null!;

    public bool IsNotificationSent { get; set; }

    public bool IsExpirationNotificationSent { get; set; }

    public int? AdvertisementId { get; set; }

    public virtual User? UserFromNavigation { get; set; }

    public virtual User UserToNavigation { get; set; } = null!;
}
