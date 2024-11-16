using FinalProject.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Data;

public partial class QlptContext : DbContext
{
    public QlptContext()
    {
    }

    public QlptContext(DbContextOptions<QlptContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FavoriteList> FavoriteLists { get; set; }

    public virtual DbSet<FavoriteListPost> FavoriteListPosts { get; set; }
    public virtual DbSet<RoomFeedback> RoomFeedbacks { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<PageAddress> PageAddresses { get; set; }

    public virtual DbSet<Privilege> Privileges { get; set; }

    public virtual DbSet<RoomImage> RoomImages { get; set; }

    public virtual DbSet<RoomPost> RoomPosts { get; set; }

    public virtual DbSet<RoomStatus> RoomStatuses { get; set; }

    public virtual DbSet<RoomType> RoomTypes { get; set; }

    public virtual DbSet<RoomUtility> RoomUtilities { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserType> UserTypes { get; set; }

    public virtual DbSet<Utility> Utilities { get; set; }
    public virtual DbSet<Response> Responses { get; set; }

    public DbSet<RoomPostVM> RoomPostVM { get; set; }

    public DbSet<RoomPostDetailVM> RoomPostDetailVM { get; set; }
    public DbSet<RoomImageVM> RoomImageVM { get; set; }
    public virtual DbSet<RoomCoordinates> RoomCoordinates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FavoriteList>(entity =>
        {
            entity.HasKey(e => e.FavoriteListId).HasName("PK__Favorite__2795432374E74827");

            entity.ToTable("Favorite_List");

            entity.Property(e => e.FavoriteListId).HasColumnName("favorite_list_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne<User>()
                  .WithOne()
                  .HasForeignKey<FavoriteList>(e => e.UserId)
                  .HasConstraintName("FK_FavoriteList_User");
        });

        modelBuilder.Entity<FavoriteListPost>(entity =>
        {
            entity.HasKey(e => e.FavoriteListPostId).HasName("PK__Favorite__C0DEC878B21639C0");

            entity.ToTable("Favorite_List_Post");

            entity.Property(e => e.FavoriteListPostId).HasColumnName("favorite_list_post_id");
            entity.Property(e => e.FavoriteId).HasColumnName("favorite_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");

            entity.HasOne(d => d.Favorite).WithMany(p => p.FavoriteListPosts)
                .HasForeignKey(d => d.FavoriteId)
                .HasConstraintName("FK__Favorite___favor__4D94879B");

            entity.HasOne(d => d.Post).WithMany(p => p.FavoriteListPosts)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__Favorite___post___4CA06362");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__7A6B2B8C61E29B7F");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId).HasColumnName("feedback_id");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.FeedbackName)
                .HasMaxLength(255)
                .HasColumnName("feedback_name");
        });

        modelBuilder.Entity<RoomFeedback>(entity =>
        {
            entity.HasKey(e => e.RoomFeedbackId).HasName("PK__Room_Fee__C4E95D155E465BEB");

            entity.ToTable("Room_Feedback");

            entity.Property(e => e.RoomFeedbackId).HasColumnName("room_feedback_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.FeedbackId).HasColumnName("feedback_id");
            entity.Property(e => e.Date).HasColumnName("feedback_date");

            entity.HasOne(d => d.User)
                .WithMany(p => p.RoomFeedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Room_Feed__user___43D61337");

            entity.HasOne(d => d.Post)
                .WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__Room_Feed__post___41EDCAC5");

            entity.HasOne(d => d.Feedback)
                .WithMany(p => p.RoomFeedbacks)
                .HasForeignKey(d => d.FeedbackId)
                .HasConstraintName("FK__Room_Feed__feedb__42E1EEFE");
        });

        modelBuilder.Entity<PageAddress>(entity =>
        {
            entity.HasKey(e => e.PageAddressId).HasName("PK__Page_Add__90439809349F8FF8");

            entity.ToTable("Page_Address");

            entity.Property(e => e.PageAddressId).HasColumnName("page_address_id");
            entity.Property(e => e.PageName)
                .HasMaxLength(100)
                .HasColumnName("page_name");
            entity.Property(e => e.Url).HasColumnName("url");
        });

        modelBuilder.Entity<Privilege>(entity =>
        {
            entity.HasKey(e => e.PrivilegeId).HasName("PK__Privileg__F94BCCE219D7F78D");

            entity.ToTable("Privilege");

            entity.Property(e => e.PrivilegeId).HasColumnName("privilege_id");
            entity.Property(e => e.IsPrivileged).HasColumnName("is_privileged");
            entity.Property(e => e.PageAddressId).HasColumnName("page_address_id");
            entity.Property(e => e.UserTypeId).HasColumnName("user_type_id");

            entity.HasOne(d => d.PageAddress).WithMany(p => p.Privileges)
                .HasForeignKey(d => d.PageAddressId)
                .HasConstraintName("FK__Privilege__page___5DCAEF64");

            entity.HasOne(d => d.UserType).WithMany(p => p.Privileges)
                .HasForeignKey(d => d.UserTypeId)
                .HasConstraintName("FK__Privilege__user___5CD6CB2B");
        });

        modelBuilder.Entity<RoomImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Room_Ima__DC9AC955A34E51B1");

            entity.ToTable("Room_Image");

            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.PostId).HasColumnName("post_id");

            entity.HasOne(d => d.Post).WithMany(p => p.RoomImages)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__Room_Imag__post___46E78A0C");
        });

        modelBuilder.Entity<RoomPost>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Room_Pos__3ED787665A274AAF");

            entity.ToTable("Room_Post");

            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.DatePosted).HasColumnName("date_posted");
            entity.Property(e => e.ExpirationDate).HasColumnName("expiration_date");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.RoomDescription).HasColumnName("room_description");
            entity.Property(e => e.RoomName)
                .HasMaxLength(100)
                .HasColumnName("room_name");
            entity.Property(e => e.RoomPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("room_price");
            entity.Property(e => e.RoomSize)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("room_size");
            entity.Property(e => e.RoomTypeId).HasColumnName("room_type_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.RoomCoordinateId).HasColumnName("room_coordinate_id");

            entity.HasOne(d => d.RoomType).WithMany(p => p.RoomPosts)
                .HasForeignKey(d => d.RoomTypeId)
                .HasConstraintName("FK__Room_Post__room___3E52440B");

            entity.HasOne(d => d.Status).WithMany(p => p.RoomPosts)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Room_Post__statu__3D5E1FD2");

            entity.HasOne(d => d.RoomCoordinate) 
                .WithMany(c => c.RoomPosts)      
                .HasForeignKey(d => d.RoomCoordinateId)
                .HasConstraintName("FK_RoomPost_Coordinates");
        });

        modelBuilder.Entity<RoomStatus>(entity =>
        {
            entity.HasKey(e => e.RoomStatusId).HasName("PK__Room_Sta__F150DB263F2981C8");

            entity.ToTable("Room_Status");

            entity.Property(e => e.RoomStatusId).HasColumnName("room_status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.RoomTypeId).HasName("PK__Room_Typ__42395E84830BCE4D");

            entity.ToTable("Room_Type");

            entity.Property(e => e.RoomTypeId).HasColumnName("room_type_id");
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<RoomUtility>(entity =>
        {
            entity.HasKey(e => e.RoomUtilityId).HasName("PK__Room_Uti__EA503ECE2C7494FC");

            entity.ToTable("Room_Utility");

            entity.Property(e => e.RoomUtilityId).HasColumnName("room_utility_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.UtilityId).HasColumnName("utility_id");

            entity.HasOne(d => d.Post).WithMany(p => p.RoomUtilities)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__Room_Util__post___412EB0B6");

            entity.HasOne(d => d.Utility).WithMany(p => p.RoomUtilities)
                .HasForeignKey(d => d.UtilityId)
                .HasConstraintName("FK__Room_Util__utili__4222D4EF");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User_New__B9BE370F18CD13D9");

            entity.ToTable("User");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(255)
                .HasColumnName("fullname");
            entity.Property(e => e.Gender)
                .HasColumnType("bit")
                .HasColumnName("gender");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.UserImage)
                .HasMaxLength(255)
                .HasColumnName("user_image");
            entity.Property(e => e.RandomKey)
                .HasMaxLength(50)
                .HasColumnName("randomkey");
            entity.Property(e => e.IsValid)
                .HasColumnName("isvalid");
            entity.Property(e => e.UserTypeId).HasColumnName("user_type_id");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
            entity.Property(e => e.Dob)
                .HasColumnType("date")
                .HasColumnName("dob");
            entity.Property(e => e.ResetToken)
                .HasMaxLength(100)
                .HasColumnName("resetToken");
            entity.Property(e => e.TokenCreateAt)
                .HasColumnType("datetime")
                .HasColumnType("tokenCreateAt");


            entity.HasOne(d => d.UserType).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserTypeId)
                .HasConstraintName("FK__User_New__user_t__72C60C4A");
        });

        modelBuilder.Entity<UserType>(entity =>
        {
            entity.HasKey(e => e.UserTypeId).HasName("PK__User_Typ__9424CFA6FDA44C1D");

            entity.ToTable("User_Type");

            entity.Property(e => e.UserTypeId).HasColumnName("user_type_id");
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<Utility>(entity =>
        {
            entity.HasKey(e => e.UtilityId).HasName("PK__Utility__3F785C70E562BA83");

            entity.ToTable("Utility");

            entity.Property(e => e.UtilityId).HasColumnName("utility_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.UtilityName)
                .HasMaxLength(50)
                .HasColumnName("utility_name");
        });

        modelBuilder.Entity<RoomCoordinates>(entity =>
        {
            entity.HasKey(e => e.RoomCoordinateId).HasName("PK__Room_Coo__4B97A2966CB5207B");

            entity.ToTable("Room_Coordinates");

            entity.Property(e => e.RoomCoordinateId).HasColumnName("room_coordinate_id");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
        });

        modelBuilder.Entity<Response>(entity =>
        {
            entity.HasKey(e => e.ResponseId).HasName("PK__Response__EBECD89617D345A5");

            entity.ToTable("Response");

            entity.Property(e => e.ResponseId).HasColumnName("response_id");
            entity.Property(e => e.RoomFeedbackId).HasColumnName("room_feedback_id");
            entity.Property(e => e.ResponseText).HasColumnName("response_text");
            entity.Property(e => e.ResponseDate).HasColumnName("response_date");

            entity.HasOne(d => d.RoomFeedback)
              .WithMany(p => p.Responses)
              .HasForeignKey(d => d.RoomFeedbackId)
              .OnDelete(DeleteBehavior.Cascade) 
              .HasConstraintName("FK__Response__room_f__4F47C5E3");
        });

        modelBuilder.Entity<RoomPostVM>(entity =>
        {
            entity.HasNoKey();
        });

        modelBuilder.Entity<RoomPostDetailVM>(entity =>
        {
            entity.HasNoKey();
        });

        modelBuilder.Entity<RoomImageVM>(entity =>
        {
            entity.HasNoKey();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
