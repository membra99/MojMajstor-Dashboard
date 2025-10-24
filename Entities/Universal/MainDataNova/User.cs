using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class User
{
    public int UsersId { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? FullName { get; set; }

    public int RoleId { get; set; }

    public int? OpstineId { get; set; }

    public string? Professions { get; set; }

    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; }

    public string? PhoneCode { get; set; }

    public bool IsVisible { get; set; }

    public string? UserToken { get; set; }

    public string ReferalCode { get; set; } = null!;

    public DateTime RegistrationDate { get; set; }

    public virtual ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();

    public virtual ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();

    public virtual ICollection<Grade> GradeUserLeftComments { get; set; } = new List<Grade>();

    public virtual ICollection<Grade> GradeUserReceiveComments { get; set; } = new List<Grade>();

    public virtual ICollection<Invitation> InvitationInvitedUsers { get; set; } = new List<Invitation>();

    public virtual ICollection<Invitation> InvitationOriginUsers { get; set; } = new List<Invitation>();

    public virtual ICollection<MakeDeal> MakeDealFirstUsers { get; set; } = new List<MakeDeal>();

    public virtual ICollection<MakeDeal> MakeDealSecondUsers { get; set; } = new List<MakeDeal>();

    public virtual ICollection<Medium> Media { get; set; } = new List<Medium>();

    public virtual ICollection<NotificationDetail> NotificationDetailUserFromNavigations { get; set; } = new List<NotificationDetail>();

    public virtual ICollection<NotificationDetail> NotificationDetailUserToNavigations { get; set; } = new List<NotificationDetail>();

    public virtual Opstine? Opstine { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<TokenUser> TokenUsers { get; set; } = new List<TokenUser>();
}
