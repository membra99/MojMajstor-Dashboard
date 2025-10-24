using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Universal.Universal.MainDataNova;

namespace Entities.Universal.MainDataNova;

public partial class MojMajstorContext : DbContext
{
    public MojMajstorContext()
    {
    }

    public MojMajstorContext(DbContextOptions<MojMajstorContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Advertisement> Advertisements { get; set; }

    public virtual DbSet<AdvertisementOpstine> AdvertisementOpstines { get; set; }

    public virtual DbSet<AdvertisementProfessionType> AdvertisementProfessionTypes { get; set; }

    public virtual DbSet<AdvertisementType> AdvertisementTypes { get; set; }

    public virtual DbSet<Bookmark> Bookmarks { get; set; }

    public virtual DbSet<DeletedUser> DeletedUsers { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Invitation> Invitations { get; set; }

    public virtual DbSet<MakeDeal> MakeDeals { get; set; }

    public virtual DbSet<MediaType> MediaTypes { get; set; }

    public virtual DbSet<Medium> Media { get; set; }

    public virtual DbSet<NotificationDetail> NotificationDetails { get; set; }

    public virtual DbSet<Opstine> Opstines { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<PayType> PayTypes { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Profession> Professions { get; set; }

    public virtual DbSet<ProfessionType> ProfessionTypes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    public virtual DbSet<TokenUser> TokenUsers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=46.38.233.135,1433;Database=MojMajstor;TrustServerCertificate=true;User Id=Sa;Password=YourStrong(!Passw0rd);");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Advertisement>(entity =>
        {
            entity.HasIndex(e => e.AdvertisementTypeId, "IX_Advertisements_AdvertisementTypeId");

            entity.HasIndex(e => e.PayTypeId, "IX_Advertisements_PayTypeId");

            entity.HasIndex(e => e.PaymentMethodId, "IX_Advertisements_PaymentMethodId");

            entity.HasIndex(e => e.ProfessionId, "IX_Advertisements_ProfessionId");

            entity.HasIndex(e => e.UsersId, "IX_Advertisements_UsersId");

            entity.HasOne(d => d.AdvertisementType).WithMany(p => p.Advertisements)
                .HasForeignKey(d => d.AdvertisementTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PayType).WithMany(p => p.Advertisements).HasForeignKey(d => d.PayTypeId);

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Advertisements).HasForeignKey(d => d.PaymentMethodId);

            entity.HasOne(d => d.Profession).WithMany(p => p.Advertisements)
                .HasForeignKey(d => d.ProfessionId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Users).WithMany(p => p.Advertisements)
                .HasForeignKey(d => d.UsersId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AdvertisementOpstine>(entity =>
        {
            entity.HasIndex(e => e.AdvertisementId, "IX_AdvertisementOpstines_AdvertisementId");

            entity.HasIndex(e => e.OpstineId, "IX_AdvertisementOpstines_OpstineId");

            entity.HasOne(d => d.Advertisement).WithMany(p => p.AdvertisementOpstines)
                .HasForeignKey(d => d.AdvertisementId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Opstine).WithMany(p => p.AdvertisementOpstines)
                .HasForeignKey(d => d.OpstineId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AdvertisementProfessionType>(entity =>
        {
            entity.HasIndex(e => e.AdvertisementId, "IX_AdvertisementProfessionTypes_AdvertisementId");

            entity.HasIndex(e => e.ProfessionTypeId, "IX_AdvertisementProfessionTypes_ProfessionTypeId");

            entity.HasOne(d => d.Advertisement).WithMany(p => p.AdvertisementProfessionTypes)
                .HasForeignKey(d => d.AdvertisementId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ProfessionType).WithMany(p => p.AdvertisementProfessionTypes)
                .HasForeignKey(d => d.ProfessionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Bookmark>(entity =>
        {
            entity.HasIndex(e => e.AdvertisementId, "IX_Bookmarks_AdvertisementId");

            entity.HasIndex(e => e.UsersId, "IX_Bookmarks_UsersId");

            entity.HasOne(d => d.Advertisement).WithMany(p => p.Bookmarks)
                .HasForeignKey(d => d.AdvertisementId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Users).WithMany(p => p.Bookmarks)
                .HasForeignKey(d => d.UsersId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DeletedUser>(entity =>
        {
            entity.HasIndex(e => e.TokenId, "IX_DeletedUsers_TokenId");

            entity.HasOne(d => d.Token).WithMany(p => p.DeletedUsers)
                .HasForeignKey(d => d.TokenId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradesId);

            entity.HasIndex(e => e.UserLeftCommentId, "IX_Grades_UserLeftCommentId");

            entity.HasIndex(e => e.UserReceiveCommentId, "IX_Grades_UserReceiveCommentId");

            entity.HasOne(d => d.UserLeftComment).WithMany(p => p.GradeUserLeftComments)
                .HasForeignKey(d => d.UserLeftCommentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UserReceiveComment).WithMany(p => p.GradeUserReceiveComments)
                .HasForeignKey(d => d.UserReceiveCommentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Invitation>(entity =>
        {
            entity.HasIndex(e => e.InvitedUserId, "IX_Invitations_InvitedUserId");

            entity.HasIndex(e => e.OriginUserId, "IX_Invitations_OriginUserId");

            entity.HasOne(d => d.InvitedUser).WithMany(p => p.InvitationInvitedUsers)
                .HasForeignKey(d => d.InvitedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OriginUser).WithMany(p => p.InvitationOriginUsers)
                .HasForeignKey(d => d.OriginUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<MakeDeal>(entity =>
        {
            entity.HasIndex(e => e.AdvertisementId, "IX_MakeDeals_AdvertisementId");

            entity.HasIndex(e => e.FirstUserId, "IX_MakeDeals_FirstUserId");

            entity.HasIndex(e => e.SecondUserId, "IX_MakeDeals_SecondUserId");

            entity.HasOne(d => d.Advertisement).WithMany(p => p.MakeDeals)
                .HasForeignKey(d => d.AdvertisementId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.FirstUser).WithMany(p => p.MakeDealFirstUsers).HasForeignKey(d => d.FirstUserId);

            entity.HasOne(d => d.SecondUser).WithMany(p => p.MakeDealSecondUsers).HasForeignKey(d => d.SecondUserId);
        });

        modelBuilder.Entity<Medium>(entity =>
        {
            entity.HasKey(e => e.MediaId);

            entity.HasIndex(e => e.MediaTypeId, "IX_Media_MediaTypeId");

            entity.HasIndex(e => e.UsersId, "IX_Media_UsersId");

            entity.Property(e => e.Src).HasColumnName("SRC");

            entity.HasOne(d => d.MediaType).WithMany(p => p.Media)
                .HasForeignKey(d => d.MediaTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Users).WithMany(p => p.Media)
                .HasForeignKey(d => d.UsersId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<NotificationDetail>(entity =>
        {
            entity.HasKey(e => e.NotificationDetailsId);

            entity.HasIndex(e => e.UserFrom, "IX_NotificationDetails_UserFrom");

            entity.HasIndex(e => e.UserTo, "IX_NotificationDetails_UserTo");

            entity.HasOne(d => d.UserFromNavigation).WithMany(p => p.NotificationDetailUserFromNavigations).HasForeignKey(d => d.UserFrom);

            entity.HasOne(d => d.UserToNavigation).WithMany(p => p.NotificationDetailUserToNavigations)
                .HasForeignKey(d => d.UserTo)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Opstine>(entity =>
        {
            entity.ToTable("Opstine");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasIndex(e => e.TokenId, "IX_Orders_TokenId");

            entity.HasIndex(e => e.UsersId, "IX_Orders_UsersId");

            entity.HasOne(d => d.Token).WithMany(p => p.Orders)
                .HasForeignKey(d => d.TokenId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Users).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UsersId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProfessionType>(entity =>
        {
            entity.HasIndex(e => e.ProfessionId, "IX_ProfessionTypes_ProfessionId");

            entity.HasOne(d => d.Profession).WithMany(p => p.ProfessionTypes)
                .HasForeignKey(d => d.ProfessionId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.Property(e => e.Description).HasDefaultValue("");
            entity.Property(e => e.IsRecommended).HasColumnName("isRecommended");
        });

        modelBuilder.Entity<TokenUser>(entity =>
        {
            entity.HasIndex(e => e.UsersId, "IX_TokenUsers_UsersId");

            entity.HasOne(d => d.Users).WithMany(p => p.TokenUsers)
                .HasForeignKey(d => d.UsersId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UsersId);

            entity.HasIndex(e => e.OpstineId, "IX_Users_OpstineId");

            entity.HasIndex(e => e.RoleId, "IX_Users_RoleId");

            entity.Property(e => e.ReferalCode).HasDefaultValue("");

            entity.HasOne(d => d.Opstine).WithMany(p => p.Users).HasForeignKey(d => d.OpstineId);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
