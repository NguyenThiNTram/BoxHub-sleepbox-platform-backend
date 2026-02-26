using BoxHub.Application.Common;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace BoxHub.Infrastructure.Data;

public partial class BoxHubDbContext : DbContext, IApplicationDbContext
{
    public BoxHubDbContext()
    {
    }

    public BoxHubDbContext(DbContextOptions<BoxHubDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AddonService> AddonServices { get; set; }

    public virtual DbSet<Amenity> Amenities { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingAddonItem> BookingAddonItems { get; set; }

    public virtual DbSet<BoxAvailability> BoxAvailabilities { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Commission> Commissions { get; set; }

    public virtual DbSet<Conversation> Conversations { get; set; }

    public virtual DbSet<Dispute> Disputes { get; set; }

    public virtual DbSet<DisputeAttachment> DisputeAttachments { get; set; }

    public virtual DbSet<Facility> Facilities { get; set; }

    public virtual DbSet<FacilityFloor> FacilityFloors { get; set; }

    public virtual DbSet<HostAddonPrice> HostAddonPrices { get; set; }

    public virtual DbSet<HostBasePrice> HostBasePrices { get; set; }

    public virtual DbSet<HostDocument> HostDocuments { get; set; }

    public virtual DbSet<HostPayout> HostPayouts { get; set; }

    public virtual DbSet<HostProfile> HostProfiles { get; set; }

    public virtual DbSet<MediaAsset> MediaAssets { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    public virtual DbSet<PricingCombo> PricingCombos { get; set; }

    public virtual DbSet<PricingFactor> PricingFactors { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Sleepbox> Sleepboxes { get; set; }

    public virtual DbSet<StaffProfile> StaffProfiles { get; set; }

    public virtual DbSet<SystemFee> SystemFees { get; set; }

    public virtual DbSet<SystemPriceRule> SystemPriceRules { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseNpgsql("Host=aws-1-ap-northeast-1.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.wijgyeanvperpkddzyyy;Password=xZXlDU4hZK27nBHA;SSL Mode=Require;Trust Server Certificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("core");
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone" })
            .HasPostgresEnum("auth", "oauth_authorization_status", new[] { "pending", "approved", "denied", "expired" })
            .HasPostgresEnum("auth", "oauth_client_type", new[] { "public", "confidential" })
            .HasPostgresEnum("auth", "oauth_registration_type", new[] { "dynamic", "manual" })
            .HasPostgresEnum("auth", "oauth_response_type", new[] { "code" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("core", "box_availability_status", new[] { "AVAILABLE", "BOOKED", "CLEANING", "MAINTENANCE" })
            .HasPostgresEnum("core", "user_role", new[] { "GUEST", "HOST", "ADMIN", "MODERATOR", "STAFF" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
            .HasPostgresEnum("storage", "buckettype", new[] { "STANDARD", "ANALYTICS", "VECTOR" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<AddonService>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("addon_services_pkey");

            entity.ToTable("addon_services", "core");

            entity.Property(e => e.ServiceId)
                .ValueGeneratedNever()
                .HasColumnName("service_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.MaxPrice).HasColumnName("max_price");
            entity.Property(e => e.MinPrice).HasColumnName("min_price");
            entity.Property(e => e.ServiceName)
                .HasColumnType("character varying")
                .HasColumnName("service_name");
            entity.Property(e => e.Unit)
                .HasColumnType("character varying")
                .HasColumnName("unit");
        });

        modelBuilder.Entity<Amenity>(entity =>
        {
            entity.HasKey(e => e.AmenityId).HasName("amenities_pkey");

            entity.ToTable("amenities", "core");

            entity.Property(e => e.AmenityId)
                .ValueGeneratedNever()
                .HasColumnName("amenity_id");
            entity.Property(e => e.AmenityCategory)
                .HasColumnType("character varying")
                .HasColumnName("amenity_category");
            entity.Property(e => e.AmenityName)
                .HasColumnType("character varying")
                .HasColumnName("amenity_name");
            entity.Property(e => e.AmenityScope)
                .HasColumnType("character varying")
                .HasColumnName("amenity_scope");
            entity.Property(e => e.AmenityType)
                .HasColumnType("character varying")
                .HasColumnName("amenity_type");
            entity.Property(e => e.IconUrl)
                .HasColumnType("character varying")
                .HasColumnName("icon_url");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("audit_logs_pkey");

            entity.ToTable("audit_logs", "core");

            entity.Property(e => e.AuditId)
                .ValueGeneratedNever()
                .HasColumnName("audit_id");
            entity.Property(e => e.Action)
                .HasColumnType("character varying")
                .HasColumnName("action");
            entity.Property(e => e.ActorId).HasColumnName("actor_id");
            entity.Property(e => e.ActorRole)
                .HasColumnType("character varying")
                .HasColumnName("actor_role");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.NewValue).HasColumnName("new_value");
            entity.Property(e => e.Note)
                .HasColumnType("character varying")
                .HasColumnName("note");
            entity.Property(e => e.OldValue).HasColumnName("old_value");
            entity.Property(e => e.TargetId).HasColumnName("target_id");
            entity.Property(e => e.TargetType)
                .HasColumnType("character varying")
                .HasColumnName("target_type");

            entity.HasOne(d => d.Actor).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.ActorId)
                .HasConstraintName("audit_logs_actor_id_fkey");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("bookings_pkey");

            entity.ToTable("bookings", "core");

            entity.HasIndex(e => e.BookingCode, "bookings_booking_code_key").IsUnique();

            entity.HasIndex(e => e.GuestId, "idx_bookings_guest");

            entity.Property(e => e.BookingId)
                .ValueGeneratedNever()
                .HasColumnName("booking_id");
            entity.Property(e => e.ActualCheckIn)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("actual_check_in");
            entity.Property(e => e.ActualCheckOut)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("actual_check_out");
            entity.Property(e => e.BookingCode)
                .HasColumnType("character varying")
                .HasColumnName("booking_code");
            entity.Property(e => e.BookingStatus)
                .HasColumnType("character varying")
                .HasColumnName("booking_status");
            entity.Property(e => e.BoxId).HasColumnName("box_id");
            entity.Property(e => e.CancelledAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledById).HasColumnName("cancelled_by_id");
            entity.Property(e => e.CheckIn)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("check_in");
            entity.Property(e => e.CheckOut)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("check_out");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FinalAmount).HasColumnName("final_amount");
            entity.Property(e => e.GuestId).HasColumnName("guest_id");
            entity.Property(e => e.PaymentStatus)
                .HasColumnType("character varying")
                .HasColumnName("payment_status");
            entity.Property(e => e.TotalAmenityPrice).HasColumnName("total_amenity_price");
            entity.Property(e => e.TotalBoxPrice).HasColumnName("total_box_price");
            entity.Property(e => e.TotalPlatformFee).HasColumnName("total_platform_fee");

            entity.HasOne(d => d.Box).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.BoxId)
                .HasConstraintName("bookings_box_id_fkey");

            entity.HasOne(d => d.CancelledBy).WithMany(p => p.BookingCancelledBies)
                .HasForeignKey(d => d.CancelledById)
                .HasConstraintName("bookings_cancelled_by_id_fkey");

            entity.HasOne(d => d.Guest).WithMany(p => p.BookingGuests)
                .HasForeignKey(d => d.GuestId)
                .HasConstraintName("bookings_guest_id_fkey");
        });

        modelBuilder.Entity<BookingAddonItem>(entity =>
        {
            entity.HasKey(e => e.BookingAddonId).HasName("booking_addon_items_pkey");

            entity.ToTable("booking_addon_items", "core");

            entity.HasIndex(e => new { e.BookingId, e.ServiceId }, "booking_addon_items_booking_id_service_id_key").IsUnique();

            entity.HasIndex(e => e.BookingId, "idx_booking_addon_booking");

            entity.Property(e => e.BookingAddonId)
                .ValueGeneratedNever()
                .HasColumnName("booking_addon_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.TotalPrice).HasColumnName("total_price");
            entity.Property(e => e.UnitPrice).HasColumnName("unit_price");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingAddonItems)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("booking_addon_items_booking_id_fkey");

            entity.HasOne(d => d.Service).WithMany(p => p.BookingAddonItems)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("booking_addon_items_service_id_fkey");
        });

        modelBuilder.Entity<BoxAvailability>(entity =>
        {
            entity.HasKey(e => e.AvailabilityId).HasName("box_availability_pkey");

            entity.ToTable("box_availability", "core");

            entity.Property(e => e.AvailabilityId)
                .ValueGeneratedNever()
                .HasColumnName("availability_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.BoxId).HasColumnName("box_id");
            entity.Property(e => e.EndTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("end_time");
            entity.Property(e => e.StartTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start_time");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Booking).WithMany(p => p.BoxAvailabilities)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("box_availability_booking_id_fkey");

            entity.HasOne(d => d.Box).WithMany(p => p.BoxAvailabilities)
                .HasForeignKey(d => d.BoxId)
                .HasConstraintName("box_availability_box_id_fkey");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("brands_pkey");

            entity.ToTable("brands", "core");

            entity.Property(e => e.BrandId)
                .ValueGeneratedNever()
                .HasColumnName("brand_id");
            entity.Property(e => e.BrandAvatar)
                .HasColumnType("character varying")
                .HasColumnName("brand_avatar");
            entity.Property(e => e.BrandName)
                .HasColumnType("character varying")
                .HasColumnName("brand_name");
            entity.Property(e => e.HostId).HasColumnName("host_id");

            entity.HasOne(d => d.Host).WithMany(p => p.Brands)
                .HasForeignKey(d => d.HostId)
                .HasConstraintName("brands_host_id_fkey");
        });

        modelBuilder.Entity<Commission>(entity =>
        {
            entity.HasKey(e => e.CommissionId).HasName("commissions_pkey");

            entity.ToTable("commissions", "core");

            entity.Property(e => e.CommissionId)
                .ValueGeneratedNever()
                .HasColumnName("commission_id");
            entity.Property(e => e.EffectiveFrom)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("effective_from");
            entity.Property(e => e.EffectiveTo)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("effective_to");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Percentage).HasColumnName("percentage");
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.ConversationId).HasName("conversations_pkey");

            entity.ToTable("conversations", "core");

            entity.Property(e => e.ConversationId)
                .ValueGeneratedNever()
                .HasColumnName("conversation_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FacilityId).HasColumnName("facility_id");
            entity.Property(e => e.GuestId).HasColumnName("guest_id");
            entity.Property(e => e.HostId).HasColumnName("host_id");
            entity.Property(e => e.LastMessageAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_message_at");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.Conversations)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("conversations_booking_id_fkey");

            entity.HasOne(d => d.Facility).WithMany(p => p.Conversations)
                .HasForeignKey(d => d.FacilityId)
                .HasConstraintName("conversations_facility_id_fkey");

            entity.HasOne(d => d.Guest).WithMany(p => p.Conversations)
                .HasForeignKey(d => d.GuestId)
                .HasConstraintName("conversations_guest_id_fkey");

            entity.HasOne(d => d.Host).WithMany(p => p.Conversations)
                .HasForeignKey(d => d.HostId)
                .HasConstraintName("conversations_host_id_fkey");

            entity.HasOne(d => d.Staff).WithMany(p => p.Conversations)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("conversations_staff_id_fkey");
        });

        modelBuilder.Entity<Dispute>(entity =>
        {
            entity.HasKey(e => e.DisputeId).HasName("disputes_pkey");

            entity.ToTable("disputes", "core");

            entity.Property(e => e.DisputeId)
                .ValueGeneratedNever()
                .HasColumnName("dispute_id");
            entity.Property(e => e.AdminApprovalId).HasColumnName("admin_approval_id");
            entity.Property(e => e.AssignedModeratorId).HasColumnName("assigned_moderator_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DisputeType)
                .HasColumnType("character varying")
                .HasColumnName("dispute_type");
            entity.Property(e => e.ModeratorNote).HasColumnName("moderator_note");
            entity.Property(e => e.RaisedBy).HasColumnName("raised_by");
            entity.Property(e => e.RefundAmount)
                .HasDefaultValueSql("0")
                .HasColumnName("refund_amount");
            entity.Property(e => e.ResolutionType)
                .HasColumnType("character varying")
                .HasColumnName("resolution_type");
            entity.Property(e => e.ResolvedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("resolved_at");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'PENDING'::character varying")
                .HasColumnType("character varying")
                .HasColumnName("status");

            entity.HasOne(d => d.AdminApproval).WithMany(p => p.DisputeAdminApprovals)
                .HasForeignKey(d => d.AdminApprovalId)
                .HasConstraintName("disputes_admin_approval_id_fkey");

            entity.HasOne(d => d.AssignedModerator).WithMany(p => p.DisputeAssignedModerators)
                .HasForeignKey(d => d.AssignedModeratorId)
                .HasConstraintName("disputes_assigned_moderator_id_fkey");

            entity.HasOne(d => d.Booking).WithMany(p => p.Disputes)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("disputes_booking_id_fkey");

            entity.HasOne(d => d.RaisedByNavigation).WithMany(p => p.DisputeRaisedByNavigations)
                .HasForeignKey(d => d.RaisedBy)
                .HasConstraintName("disputes_raised_by_fkey");
        });

        modelBuilder.Entity<DisputeAttachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId).HasName("dispute_attachments_pkey");

            entity.ToTable("dispute_attachments", "core");

            entity.Property(e => e.AttachmentId)
                .ValueGeneratedNever()
                .HasColumnName("attachment_id");
            entity.Property(e => e.AttachmentRole)
                .HasColumnType("character varying")
                .HasColumnName("attachment_role");
            entity.Property(e => e.DisputeId).HasColumnName("dispute_id");
            entity.Property(e => e.FileType)
                .HasColumnType("character varying")
                .HasColumnName("file_type");
            entity.Property(e => e.FileUrl)
                .HasColumnType("character varying")
                .HasColumnName("file_url");
            entity.Property(e => e.UploadedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("uploaded_at");
            entity.Property(e => e.UploadedBy).HasColumnName("uploaded_by");

            entity.HasOne(d => d.Dispute).WithMany(p => p.DisputeAttachments)
                .HasForeignKey(d => d.DisputeId)
                .HasConstraintName("dispute_attachments_dispute_id_fkey");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.DisputeAttachments)
                .HasForeignKey(d => d.UploadedBy)
                .HasConstraintName("dispute_attachments_uploaded_by_fkey");
        });

        modelBuilder.Entity<Facility>(entity =>
        {
            entity.HasKey(e => e.FacilityId).HasName("facilities_pkey");

            entity.ToTable("facilities", "core");

            entity.Property(e => e.FacilityId)
                .ValueGeneratedNever()
                .HasColumnName("facility_id");
            entity.Property(e => e.AddressCity)
                .HasColumnType("character varying")
                .HasColumnName("address_city");
            entity.Property(e => e.AddressDistrict)
                .HasColumnType("character varying")
                .HasColumnName("address_district");
            entity.Property(e => e.AddressStreet)
                .HasColumnType("character varying")
                .HasColumnName("address_street");
            entity.Property(e => e.AddressWard)
                .HasColumnType("character varying")
                .HasColumnName("address_ward");
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FacilityName)
                .HasColumnType("character varying")
                .HasColumnName("facility_name");
            entity.Property(e => e.FacilityStatus)
                .HasColumnType("character varying")
                .HasColumnName("facility_status");
            entity.Property(e => e.HouseRules).HasColumnName("house_rules");
            entity.Property(e => e.Latitude)
                .HasPrecision(11, 9)
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasPrecision(11, 9)
                .HasColumnName("longitude");

            entity.HasOne(d => d.Brand).WithMany(p => p.Facilities)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("facilities_brand_id_fkey");
        });

        modelBuilder.Entity<FacilityFloor>(entity =>
        {
            entity.HasKey(e => e.FloorId).HasName("facility_floors_pkey");

            entity.ToTable("facility_floors", "core");

            entity.Property(e => e.FloorId)
                .ValueGeneratedNever()
                .HasColumnName("floor_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FacilityId).HasColumnName("facility_id");
            entity.Property(e => e.FloorAmenities)
                .HasColumnType("json")
                .HasColumnName("floor_amenities");
            entity.Property(e => e.FloorName)
                .HasColumnType("character varying")
                .HasColumnName("floor_name");

            entity.HasOne(d => d.Facility).WithMany(p => p.FacilityFloors)
                .HasForeignKey(d => d.FacilityId)
                .HasConstraintName("facility_floors_facility_id_fkey");
        });

        modelBuilder.Entity<HostAddonPrice>(entity =>
        {
            entity.HasKey(e => e.HostServiceId).HasName("host_addon_prices_pkey");

            entity.ToTable("host_addon_prices", "core");

            entity.Property(e => e.HostServiceId)
                .ValueGeneratedNever()
                .HasColumnName("host_service_id");
            entity.Property(e => e.FacilityId).HasColumnName("facility_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");

            entity.HasOne(d => d.Facility).WithMany(p => p.HostAddonPrices)
                .HasForeignKey(d => d.FacilityId)
                .HasConstraintName("host_addon_prices_facility_id_fkey");

            entity.HasOne(d => d.Service).WithMany(p => p.HostAddonPrices)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("host_addon_prices_service_id_fkey");
        });

        modelBuilder.Entity<HostBasePrice>(entity =>
        {
            entity.HasKey(e => e.HostPriceId).HasName("host_base_prices_pkey");

            entity.ToTable("host_base_prices", "core");

            entity.Property(e => e.HostPriceId)
                .ValueGeneratedNever()
                .HasColumnName("host_price_id");
            entity.Property(e => e.AppliedLocationCode)
                .HasColumnType("character varying")
                .HasColumnName("applied_location_code");
            entity.Property(e => e.BaseHourPrice).HasColumnName("base_hour_price");
            entity.Property(e => e.BaseOvernightPrice).HasColumnName("base_overnight_price");
            entity.Property(e => e.BoxType)
                .HasColumnType("character varying")
                .HasColumnName("box_type");
            entity.Property(e => e.FacilityId).HasColumnName("facility_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");

            entity.HasOne(d => d.Facility).WithMany(p => p.HostBasePrices)
                .HasForeignKey(d => d.FacilityId)
                .HasConstraintName("host_base_prices_facility_id_fkey");
        });

        modelBuilder.Entity<HostDocument>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("host_documents_pkey");

            entity.ToTable("host_documents", "core");

            entity.Property(e => e.DocumentId)
                .ValueGeneratedNever()
                .HasColumnName("document_id");
            entity.Property(e => e.DocumentStatus)
                .HasColumnType("character varying")
                .HasColumnName("document_status");
            entity.Property(e => e.DocumentType)
                .HasColumnType("character varying")
                .HasColumnName("document_type");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.FileName)
                .HasColumnType("character varying")
                .HasColumnName("file_name");
            entity.Property(e => e.FileType)
                .HasColumnType("character varying")
                .HasColumnName("file_type");
            entity.Property(e => e.FileUrl)
                .HasColumnType("character varying")
                .HasColumnName("file_url");
            entity.Property(e => e.HostId).HasColumnName("host_id");
            entity.Property(e => e.RejectReason)
                .HasColumnType("character varying")
                .HasColumnName("reject_reason");
            entity.Property(e => e.ReviewedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("reviewed_at");
            entity.Property(e => e.ReviewedBy).HasColumnName("reviewed_by");
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("uploaded_at");

            entity.HasOne(d => d.Host).WithMany(p => p.HostDocuments)
                .HasForeignKey(d => d.HostId)
                .HasConstraintName("host_documents_host_id_fkey");

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.HostDocuments)
                .HasForeignKey(d => d.ReviewedBy)
                .HasConstraintName("host_documents_reviewed_by_fkey");
        });

        modelBuilder.Entity<HostPayout>(entity =>
        {
            entity.HasKey(e => e.PayoutId).HasName("host_payouts_pkey");

            entity.ToTable("host_payouts", "core");

            entity.Property(e => e.PayoutId)
                .ValueGeneratedNever()
                .HasColumnName("payout_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CommissionAmount).HasColumnName("commission_amount");
            entity.Property(e => e.CommissionId).HasColumnName("commission_id");
            entity.Property(e => e.GrossAmount).HasColumnName("gross_amount");
            entity.Property(e => e.HostId).HasColumnName("host_id");
            entity.Property(e => e.NetAmount).HasColumnName("net_amount");
            entity.Property(e => e.PaidAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("paid_at");
            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.PayoutStatus)
                .HasColumnType("character varying")
                .HasColumnName("payout_status");

            entity.HasOne(d => d.Booking).WithMany(p => p.HostPayouts)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("host_payouts_booking_id_fkey");

            entity.HasOne(d => d.Commission).WithMany(p => p.HostPayouts)
                .HasForeignKey(d => d.CommissionId)
                .HasConstraintName("host_payouts_commission_id_fkey");

            entity.HasOne(d => d.Host).WithMany(p => p.HostPayouts)
                .HasForeignKey(d => d.HostId)
                .HasConstraintName("host_payouts_host_id_fkey");

            entity.HasOne(d => d.Payment).WithMany(p => p.HostPayouts)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("host_payouts_payment_id_fkey");
        });

        modelBuilder.Entity<HostProfile>(entity =>
        {
            entity.HasKey(e => e.HostId).HasName("host_profiles_pkey");

            entity.ToTable("host_profiles", "core");

            entity.HasIndex(e => e.UserId, "host_profiles_user_id_key").IsUnique();

            entity.Property(e => e.HostId)
                .ValueGeneratedNever()
                .HasColumnName("host_id");
            entity.Property(e => e.BrandName)
                .HasColumnType("character varying")
                .HasColumnName("brand_name");
            entity.Property(e => e.BusinessAddress)
                .HasColumnType("character varying")
                .HasColumnName("business_address");
            entity.Property(e => e.BusinessCity)
                .HasColumnType("character varying")
                .HasColumnName("business_city");
            entity.Property(e => e.BusinessDistrict)
                .HasColumnType("character varying")
                .HasColumnName("business_district");
            entity.Property(e => e.BusinessWard)
                .HasColumnType("character varying")
                .HasColumnName("business_ward");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.RepresentativeIdNumber)
                .HasColumnType("character varying")
                .HasColumnName("representative_id_number");
            entity.Property(e => e.RepresentativeName)
                .HasColumnType("character varying")
                .HasColumnName("representative_name");
            entity.Property(e => e.TaxCode)
                .HasColumnType("character varying")
                .HasColumnName("tax_code");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VerifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("verified_at");
            entity.Property(e => e.VerifiedStatus)
                .HasDefaultValueSql("'PENDING'::character varying")
                .HasColumnType("character varying")
                .HasColumnName("verified_status");

            entity.HasOne(d => d.User).WithOne(p => p.HostProfile)
                .HasForeignKey<HostProfile>(d => d.UserId)
                .HasConstraintName("host_profiles_user_id_fkey");
        });

        modelBuilder.Entity<MediaAsset>(entity =>
        {
            entity.HasKey(e => e.MediaId).HasName("media_assets_pkey");

            entity.ToTable("media_assets", "core");

            entity.Property(e => e.MediaId)
                .ValueGeneratedNever()
                .HasColumnName("media_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
            entity.Property(e => e.IsCover).HasColumnName("is_cover");
            entity.Property(e => e.MediaType)
                .HasColumnType("character varying")
                .HasColumnName("media_type");
            entity.Property(e => e.MediaUrl)
                .HasColumnType("character varying")
                .HasColumnName("media_url");
            entity.Property(e => e.TargetId).HasColumnName("target_id");
            entity.Property(e => e.ThumbnailUrl)
                .HasColumnType("character varying")
                .HasColumnName("thumbnail_url");

            entity.HasOne(d => d.Target).WithMany(p => p.MediaAssets)
                .HasForeignKey(d => d.TargetId)
                .HasConstraintName("media_assets_target_id_fkey");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("messages_pkey");

            entity.ToTable("messages", "core");

            entity.Property(e => e.MessageId)
                .ValueGeneratedNever()
                .HasColumnName("message_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.SentAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("sent_at");

            entity.HasOne(d => d.Conversation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ConversationId)
                .HasConstraintName("messages_conversation_id_fkey");

            entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("messages_sender_id_fkey");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("notifications_pkey");

            entity.ToTable("notifications", "core");

            entity.Property(e => e.NotificationId)
                .ValueGeneratedNever()
                .HasColumnName("notification_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.NotificationType)
                .HasColumnType("character varying")
                .HasColumnName("notification_type");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("notifications_user_id_fkey");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("payments_pkey");

            entity.ToTable("payments", "core");

            entity.HasIndex(e => e.BookingId, "idx_payments_booking");

            entity.HasIndex(e => e.ClientRequestId, "payments_client_request_id_key").IsUnique();

            entity.Property(e => e.PaymentId)
                .ValueGeneratedNever()
                .HasColumnName("payment_id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.ClientRequestId)
                .HasColumnType("character varying")
                .HasColumnName("client_request_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.PaymentStatus)
                .HasColumnType("character varying")
                .HasColumnName("payment_status");
            entity.Property(e => e.PaymentType)
                .HasColumnType("character varying")
                .HasColumnName("payment_type");
            entity.Property(e => e.ReferenceNote)
                .HasColumnType("character varying")
                .HasColumnName("reference_note");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("payments_booking_id_fkey");
        });

        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("payment_transactions_pkey");

            entity.ToTable("payment_transactions", "core");

            entity.Property(e => e.TransactionId)
                .ValueGeneratedNever()
                .HasColumnName("transaction_id");
            entity.Property(e => e.CallbackPayload).HasColumnName("callback_payload");
            entity.Property(e => e.CallbackReceivedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("callback_received_at");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Environment)
                .HasColumnType("character varying")
                .HasColumnName("environment");
            entity.Property(e => e.GatewayTransactionId)
                .HasColumnType("character varying")
                .HasColumnName("gateway_transaction_id");
            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Provider)
                .HasColumnType("character varying")
                .HasColumnName("provider");
            entity.Property(e => e.RequestType)
                .HasColumnType("character varying")
                .HasColumnName("request_type");
            entity.Property(e => e.TransactionStatus)
                .HasColumnType("character varying")
                .HasColumnName("transaction_status");

            entity.HasOne(d => d.Payment).WithMany(p => p.PaymentTransactions)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("payment_transactions_payment_id_fkey");
        });

        modelBuilder.Entity<PricingCombo>(entity =>
        {
            entity.HasKey(e => e.ComboId).HasName("pricing_combos_pkey");

            entity.ToTable("pricing_combos", "core");

            entity.Property(e => e.ComboId)
                .ValueGeneratedNever()
                .HasColumnName("combo_id");
            entity.Property(e => e.ComboFactor)
                .HasPrecision(3, 2)
                .HasColumnName("combo_factor");
            entity.Property(e => e.Hours).HasColumnName("hours");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
        });

        modelBuilder.Entity<PricingFactor>(entity =>
        {
            entity.HasKey(e => e.FactorId).HasName("pricing_factors_pkey");

            entity.ToTable("pricing_factors", "core");

            entity.Property(e => e.FactorId)
                .ValueGeneratedNever()
                .HasColumnName("factor_id");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.FactorType)
                .HasColumnType("character varying")
                .HasColumnName("factor_type");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.MaxFactor)
                .HasPrecision(3, 2)
                .HasColumnName("max_factor");
            entity.Property(e => e.MinFactor)
                .HasPrecision(3, 2)
                .HasColumnName("min_factor");
            entity.Property(e => e.Priority)
                .HasDefaultValue(0)
                .HasColumnName("priority");
            entity.Property(e => e.RefCode)
                .HasColumnType("character varying")
                .HasColumnName("ref_code");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("reviews_pkey");

            entity.ToTable("reviews", "core");

            entity.HasIndex(e => e.BookingId, "reviews_booking_id_key").IsUnique();

            entity.Property(e => e.ReviewId)
                .ValueGeneratedNever()
                .HasColumnName("review_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.BoxId).HasColumnName("box_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.GuestId).HasColumnName("guest_id");
            entity.Property(e => e.RatingScore).HasColumnName("rating_score");
            entity.Property(e => e.ReviewStatus)
                .HasColumnType("character varying")
                .HasColumnName("review_status");

            entity.HasOne(d => d.Booking).WithOne(p => p.Review)
                .HasForeignKey<Review>(d => d.BookingId)
                .HasConstraintName("reviews_booking_id_fkey");

            entity.HasOne(d => d.Box).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BoxId)
                .HasConstraintName("reviews_box_id_fkey");

            entity.HasOne(d => d.Guest).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.GuestId)
                .HasConstraintName("reviews_guest_id_fkey");
        });

        modelBuilder.Entity<Sleepbox>(entity =>
        {
            entity.HasKey(e => e.BoxId).HasName("sleepbox_pkey");

            entity.ToTable("sleepbox", "core");

            entity.Property(e => e.BoxId)
                .ValueGeneratedNever()
                .HasColumnName("box_id");
            entity.Property(e => e.BoxLevel)
                .HasColumnType("character varying")
                .HasColumnName("box_level");
            entity.Property(e => e.BoxName)
                .HasColumnType("character varying")
                .HasColumnName("box_name");
            entity.Property(e => e.BoxRow)
                .HasColumnType("character varying")
                .HasColumnName("box_row");
            entity.Property(e => e.BoxStatus)
                .HasColumnType("character varying")
                .HasColumnName("box_status");
            entity.Property(e => e.BoxType)
                .HasColumnType("character varying")
                .HasColumnName("box_type");
            entity.Property(e => e.CleaningBufferMinutes).HasColumnName("cleaning_buffer_minutes");
            entity.Property(e => e.FloorId).HasColumnName("floor_id");
            entity.Property(e => e.SizeHeight).HasColumnName("size_height");
            entity.Property(e => e.SizeLength).HasColumnName("size_length");
            entity.Property(e => e.SizeWidth).HasColumnName("size_width");

            entity.HasOne(d => d.Floor).WithMany(p => p.Sleepboxes)
                .HasForeignKey(d => d.FloorId)
                .HasConstraintName("sleepbox_floor_id_fkey");

            entity.HasMany(d => d.Amenities).WithMany(p => p.Boxes)
                .UsingEntity<Dictionary<string, object>>(
                    "SleepboxAmenity",
                    r => r.HasOne<Amenity>().WithMany()
                        .HasForeignKey("AmenityId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("sleepbox_amenities_amenity_id_fkey"),
                    l => l.HasOne<Sleepbox>().WithMany()
                        .HasForeignKey("BoxId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("sleepbox_amenities_box_id_fkey"),
                    j =>
                    {
                        j.HasKey("BoxId", "AmenityId").HasName("sleepbox_amenities_pkey");
                        j.ToTable("sleepbox_amenities", "core");
                        j.IndexerProperty<Guid>("BoxId").HasColumnName("box_id");
                        j.IndexerProperty<int>("AmenityId").HasColumnName("amenity_id");
                    });
        });

        modelBuilder.Entity<StaffProfile>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("staff_profiles_pkey");

            entity.ToTable("staff_profiles", "core");

            entity.Property(e => e.StaffId)
                .ValueGeneratedNever()
                .HasColumnName("staff_id");
            entity.Property(e => e.FacilityId).HasColumnName("facility_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.JobDescription).HasColumnName("job_description");
            entity.Property(e => e.Position)
                .HasColumnType("character varying")
                .HasColumnName("position");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WorkplaceNote)
                .HasColumnType("character varying")
                .HasColumnName("workplace_note");

            entity.HasOne(d => d.Facility).WithMany(p => p.StaffProfiles)
                .HasForeignKey(d => d.FacilityId)
                .HasConstraintName("staff_profiles_facility_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.StaffProfiles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("staff_profiles_user_id_fkey");
        });

        modelBuilder.Entity<SystemFee>(entity =>
        {
            entity.HasKey(e => e.SystemFeeId).HasName("system_fees_pkey");

            entity.ToTable("system_fees", "core");

            entity.Property(e => e.SystemFeeId)
                .ValueGeneratedNever()
                .HasColumnName("system_fee_id");
            entity.Property(e => e.EffectiveFrom)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("effective_from");
            entity.Property(e => e.EffectiveTo)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("effective_to");
            entity.Property(e => e.FeeCode)
                .HasColumnType("character varying")
                .HasColumnName("fee_code");
            entity.Property(e => e.FeeType)
                .HasColumnType("character varying")
                .HasColumnName("fee_type");
            entity.Property(e => e.FeeValue).HasColumnName("fee_value");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
        });

        modelBuilder.Entity<SystemPriceRule>(entity =>
        {
            entity.HasKey(e => e.RuleId).HasName("system_price_rules_pkey");

            entity.ToTable("system_price_rules", "core");

            entity.Property(e => e.RuleId)
                .ValueGeneratedNever()
                .HasColumnName("rule_id");
            entity.Property(e => e.BoxType)
                .HasColumnType("character varying")
                .HasColumnName("box_type");
            entity.Property(e => e.FixedEndTime).HasColumnName("fixed_end_time");
            entity.Property(e => e.FixedStartTime).HasColumnName("fixed_start_time");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.MaxHours).HasColumnName("max_hours");
            entity.Property(e => e.MaxPrice).HasColumnName("max_price");
            entity.Property(e => e.MinHours).HasColumnName("min_hours");
            entity.Property(e => e.MinPrice).HasColumnName("min_price");
            entity.Property(e => e.PricingMode)
                .HasColumnType("character varying")
                .HasColumnName("pricing_mode");
        });

        modelBuilder.HasPostgresEnum<UserRole>("core", "user_role");

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users", "core");

            entity.HasIndex(e => e.Email, "idx_users_email");

            entity.HasIndex(e => e.Username, "idx_users_username");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.EmailVerifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("email_verified_at");
            entity.Property(e => e.IsEmailVerified)
                .HasDefaultValue(false)
                .HasColumnName("is_email_verified");
            entity.Property(e => e.LastLoginAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_login_at");
            entity.Property(e => e.PasswordHash)
                .HasColumnType("character varying")
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasColumnType("character varying")
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasColumnName("role")
                .HasColumnType("core.user_role")
                .HasConversion<UserRole>();
            entity.Property(e => e.UserStatus)
                .HasDefaultValueSql("'ACTIVE'::character varying")
                .HasColumnType("character varying")
                .HasColumnName("user_status");
            entity.Property(e => e.Username)
                .HasColumnType("character varying")
                .HasColumnName("username");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("user_profiles_pkey");

            entity.ToTable("user_profiles", "core");

            entity.HasIndex(e => e.UserId, "user_profiles_user_id_key").IsUnique();

            entity.Property(e => e.ProfileId)
                .ValueGeneratedNever()
                .HasColumnName("profile_id");
            entity.Property(e => e.AvatarUrl)
                .HasColumnType("character varying")
                .HasColumnName("avatar_url");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.FirstName)
                .HasColumnType("character varying")
                .HasColumnName("first_name");
            entity.Property(e => e.Gender)
                .HasColumnType("character varying")
                .HasColumnName("gender");
            entity.Property(e => e.LastName)
                .HasColumnType("character varying")
                .HasColumnName("last_name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.UserProfile)
                .HasForeignKey<UserProfile>(d => d.UserId)
                .HasConstraintName("user_profiles_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
