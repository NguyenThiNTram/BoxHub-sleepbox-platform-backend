using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums;
using BoxHub.Infrastructure.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace BoxHub.Infrastructure.Data;

public partial class BoxHubDbContext : DbContext
{
    public BoxHubDbContext(DbContextOptions<BoxHubDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<addon_service> addon_services { get; set; }

    public virtual DbSet<amenity> amenities { get; set; }

    public virtual DbSet<audit_log> audit_logs { get; set; }

    public virtual DbSet<booking> bookings { get; set; }

    public virtual DbSet<booking_addon_service> booking_addon_services { get; set; }

    public virtual DbSet<booking_box> booking_boxes { get; set; }

    public virtual DbSet<booking_status_history> booking_status_histories { get; set; }

    public virtual DbSet<box_availability> box_availabilities { get; set; }

    public virtual DbSet<brand> brands { get; set; }

    public virtual DbSet<conversation> conversations { get; set; }

    public virtual DbSet<dispute> disputes { get; set; }

    public virtual DbSet<dispute_attachment> dispute_attachments { get; set; }

    public virtual DbSet<email_otp> email_otps { get; set; }

    public virtual DbSet<facility> facilities { get; set; }

    public virtual DbSet<facility_area> facility_areas { get; set; }

    public virtual DbSet<facility_document> facility_documents { get; set; }

    public virtual DbSet<host_addon_price> host_addon_prices { get; set; }

    public virtual DbSet<host_base_price> host_base_prices { get; set; }

    public virtual DbSet<host_document> host_documents { get; set; }

    public virtual DbSet<host_payout_account> host_payout_accounts { get; set; }

    public virtual DbSet<host_profile> host_profiles { get; set; }

    public virtual DbSet<host_registration_draft> host_registration_drafts { get; set; }

    public virtual DbSet<media_asset> media_assets { get; set; }

    public virtual DbSet<message> messages { get; set; }

    public virtual DbSet<notification> notifications { get; set; }

    public virtual DbSet<payment> payments { get; set; }

    public virtual DbSet<payment_transaction> payment_transactions { get; set; }

    public virtual DbSet<platform_fee_config> platform_fee_configs { get; set; }

    public virtual DbSet<pricing_combo> pricing_combos { get; set; }

    public virtual DbSet<pricing_factor> pricing_factors { get; set; }

    public virtual DbSet<review> reviews { get; set; }

    public virtual DbSet<sleepbox> sleepboxes { get; set; }

    public virtual DbSet<staff_profile> staff_profiles { get; set; }

    public virtual DbSet<system_policy> system_policies { get; set; }

    public virtual DbSet<system_price_rule> system_price_rules { get; set; }

    public virtual DbSet<user> users { get; set; }

    public virtual DbSet<user_favorite> user_favorites { get; set; }

    public virtual DbSet<user_profile> user_profiles { get; set; }

    public virtual DbSet<wallet> wallets { get; set; }

    public virtual DbSet<wallet_transaction> wallet_transactions { get; set; }

    public virtual DbSet<withdrawal_request> withdrawal_requests { get; set; }

    public virtual DbSet<facility_amenity> facility_amenities { get; set; }
    public virtual DbSet<sleepbox_amenity> sleepbox_amenities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
            .HasPostgresEnum("storage", "buckettype", new[] { "STANDARD", "ANALYTICS", "VECTOR" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("btree_gist")
            .HasPostgresExtension("vault", "supabase_vault");



        modelBuilder.Entity<addon_service>(entity =>
        {
            entity.HasKey(e => e.service_id).HasName("addon_services_pkey");

            entity.Property(e => e.service_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.service_name).HasColumnType("character varying");
            entity.Property(e => e.unit).HasColumnType("character varying");
        });

        modelBuilder.Entity<amenity>(entity =>
        {
            entity.HasKey(e => e.amenity_id).HasName("amenities_pkey");

            entity.HasIndex(e => e.amenity_name, "amenities_amenity_name_key").IsUnique();

            entity.Property(e => e.amenity_name).HasColumnType("character varying");
            entity.Property(e => e.amenity_type).HasColumnType("character varying");
        });

        modelBuilder.Entity<audit_log>(entity =>
        {
            entity.HasKey(e => e.audit_id).HasName("audit_logs_pkey");

            entity.HasIndex(e => new { e.target_type, e.target_id }, "idx_audit_logs_target");

            entity.Property(e => e.audit_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.action).HasColumnType("character varying");
            entity.Property(e => e.actor_role).HasColumnType("character varying");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.note).HasColumnType("character varying");
            entity.Property(e => e.target_type).HasColumnType("character varying");

            entity.HasOne(d => d.actor).WithMany(p => p.audit_logs)
                .HasForeignKey(d => d.actor_id)
                .HasConstraintName("audit_logs_actor_id_fkey");
        });

        modelBuilder.Entity<booking>(entity =>
        {
            entity.HasKey(e => e.booking_id).HasName("bookings_pkey");

            entity.HasIndex(e => e.booking_code, "bookings_booking_code_key").IsUnique();

            entity.HasIndex(e => e.check_in, "idx_bookings_check_in");

            entity.HasIndex(e => e.check_out, "idx_bookings_check_out");

            entity.HasIndex(e => new { e.facility_id, e.check_in }, "idx_bookings_facility_check_in");

            entity.HasIndex(e => new { e.guest_id, e.created_at }, "idx_bookings_guest_created");

            entity.HasIndex(e => new { e.host_id, e.created_at }, "idx_bookings_host_created");

            entity.HasIndex(e => e.booking_status, "idx_bookings_status");

            entity.Property(e => e.booking_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.booking_code).HasColumnType("character varying");
            entity.Property(e => e.booking_status).HasColumnType("character varying");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.payment_status).HasColumnType("character varying");
            entity.Property(e => e.pricing_snapshot).HasColumnType("jsonb");

            entity.HasOne(d => d.cancelled_by).WithMany(p => p.bookingcancelled_bies)
                .HasForeignKey(d => d.cancelled_by_id)
                .HasConstraintName("bookings_cancelled_by_id_fkey");

            entity.HasOne(d => d.facility).WithMany(p => p.bookings)
                .HasForeignKey(d => d.facility_id)
                .HasConstraintName("bookings_facility_id_fkey");

            entity.HasOne(d => d.guest).WithMany(p => p.bookingguests)
                .HasForeignKey(d => d.guest_id)
                .HasConstraintName("bookings_guest_id_fkey");

            entity.HasOne(d => d.host).WithMany(p => p.bookings)
                .HasForeignKey(d => d.host_id)
                .HasConstraintName("bookings_host_id_fkey");
        });

        modelBuilder.Entity<booking_addon_service>(entity =>
        {
            entity.HasKey(e => e.booking_addon_id).HasName("booking_addon_services_pkey");

            entity.HasIndex(e => e.booking_id, "idx_booking_addons_booking");

            entity.Property(e => e.booking_addon_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.service_name).HasColumnType("character varying");

            entity.HasOne(d => d.booking).WithMany(p => p.booking_addon_services)
                .HasForeignKey(d => d.booking_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("booking_addon_services_booking_id_fkey");

            entity.HasOne(d => d.service).WithMany(p => p.booking_addon_services)
                .HasForeignKey(d => d.service_id)
                .HasConstraintName("booking_addon_services_service_id_fkey");
        });

        modelBuilder.Entity<booking_box>(entity =>
        {
            entity.HasKey(e => e.booking_box_id).HasName("booking_boxes_pkey");

            entity.HasIndex(e => e.booking_id, "idx_booking_boxes_booking");

            entity.HasIndex(e => e.box_id, "idx_booking_boxes_box");

            entity.Property(e => e.booking_box_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.box_name_snapshot).HasColumnType("character varying");
            entity.Property(e => e.box_type_snapshot).HasColumnType("character varying");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.booking).WithMany(p => p.booking_boxes)
                .HasForeignKey(d => d.booking_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("booking_boxes_booking_id_fkey");

            entity.HasOne(d => d.box).WithMany(p => p.booking_boxes)
                .HasForeignKey(d => d.box_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("booking_boxes_box_id_fkey");
        });

        modelBuilder.Entity<booking_status_history>(entity =>
        {
            entity.HasKey(e => e.history_id).HasName("booking_status_history_pkey");

            entity.ToTable("booking_status_history");

            entity.HasIndex(e => new { e.booking_id, e.changed_at }, "idx_booking_status_history_booking").IsDescending(false, true);

            entity.Property(e => e.history_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.changed_at).HasDefaultValueSql("now()");
            entity.Property(e => e.new_status).HasColumnType("character varying");
            entity.Property(e => e.note).HasColumnType("character varying");
            entity.Property(e => e.old_status).HasColumnType("character varying");

            entity.HasOne(d => d.booking).WithMany(p => p.booking_status_histories)
                .HasForeignKey(d => d.booking_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("booking_status_history_booking_id_fkey");

            entity.HasOne(d => d.payment).WithMany(p => p.booking_status_histories)
                .HasForeignKey(d => d.payment_id)
                .HasConstraintName("booking_status_history_payment_id_fkey");
        });

        modelBuilder.Entity<box_availability>(entity =>
        {
            entity.HasKey(e => e.availability_id).HasName("box_availability_pkey");

            entity.ToTable("box_availability");

            entity.HasIndex(e => new { e.facility_id, e.start_time, e.end_time }, "idx_box_availability_facility_time");

            entity.HasIndex(e => new { e.availability_status, e.locked_until }, "idx_box_availability_status");

            entity.Property(e => e.availability_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.availability_status).HasColumnType("character varying");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.booking).WithMany(p => p.box_availabilities)
                .HasForeignKey(d => d.booking_id)
                .HasConstraintName("fk_box_availability_booking");

            entity.HasOne(d => d.box).WithMany(p => p.box_availabilities)
                .HasForeignKey(d => d.box_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_box_availability_box");

            entity.HasOne(d => d.facility).WithMany(p => p.box_availabilities)
                .HasForeignKey(d => d.facility_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_box_availability_facility");
        });

        modelBuilder.Entity<brand>(entity =>
        {
            entity.HasKey(e => e.brand_id).HasName("brands_pkey");

            entity.HasIndex(e => e.host_id, "uq_active_brands_host_id").IsUnique()
                .HasFilter("is_deleted = false");
            entity.HasIndex(e => e.brand_name, "uq_active_brands_brand_name").IsUnique()
                .HasFilter("is_deleted = false");
            entity.Property(e => e.brand_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.brand_avatar).HasColumnType("character varying");
            entity.Property(e => e.brand_name).HasColumnType("character varying");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");

            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.status).HasDefaultValueSql("'PENDING'::character varying")
                .HasConversion(
                    v => v.ToString().ToUpperInvariant(),
                    v => Enum.Parse<BrandStatus>(v, true));

            entity.HasOne(d => d.host).WithOne(p => p.brand)
                .HasForeignKey<brand>(d => d.host_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("brands_host_id_fkey");
        });

        modelBuilder.Entity<conversation>(entity =>
        {
            entity.HasKey(e => e.conversation_id).HasName("conversations_pkey");

            entity.HasIndex(e => e.guest_id, "idx_conversations_guest");

            entity.HasIndex(e => e.host_id, "idx_conversations_host");

            entity.HasIndex(e => e.staff_id, "idx_conversations_staff");

            entity.Property(e => e.conversation_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.conversation_type).HasColumnType("character varying");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.booking).WithMany(p => p.conversations)
                .HasForeignKey(d => d.booking_id)
                .HasConstraintName("conversations_booking_id_fkey");

            entity.HasOne(d => d.facility).WithMany(p => p.conversations)
                .HasForeignKey(d => d.facility_id)
                .HasConstraintName("conversations_facility_id_fkey");

            entity.HasOne(d => d.guest).WithMany(p => p.conversations)
                .HasForeignKey(d => d.guest_id)
                .HasConstraintName("conversations_guest_id_fkey");

            entity.HasOne(d => d.host).WithMany(p => p.conversations)
                .HasForeignKey(d => d.host_id)
                .HasConstraintName("conversations_host_id_fkey");

            entity.HasOne(d => d.staff).WithMany(p => p.conversations)
                .HasForeignKey(d => d.staff_id)
                .HasConstraintName("conversations_staff_id_fkey");
        });

        modelBuilder.Entity<dispute>(entity =>
        {
            entity.HasKey(e => e.dispute_id).HasName("disputes_pkey");

            entity.Property(e => e.dispute_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.dispute_type).HasColumnType("character varying");
            entity.Property(e => e.refund_amount)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("0");
            entity.Property(e => e.resolution_type).HasColumnType("character varying");
            entity.Property(e => e.status)
                .HasDefaultValueSql("'PENDING'::character varying")
                .HasColumnType("character varying");

            entity.HasOne(d => d.admin_approval).WithMany(p => p.disputeadmin_approvals)
                .HasForeignKey(d => d.admin_approval_id)
                .HasConstraintName("disputes_admin_approval_id_fkey");

            entity.HasOne(d => d.assigned_moderator).WithMany(p => p.disputeassigned_moderators)
                .HasForeignKey(d => d.assigned_moderator_id)
                .HasConstraintName("disputes_assigned_moderator_id_fkey");

            entity.HasOne(d => d.booking).WithMany(p => p.disputes)
                .HasForeignKey(d => d.booking_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("disputes_booking_id_fkey");

            entity.HasOne(d => d.raised_byNavigation).WithMany(p => p.disputeraised_byNavigations)
                .HasForeignKey(d => d.raised_by)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("disputes_raised_by_fkey");
        });

        modelBuilder.Entity<dispute_attachment>(entity =>
        {
            entity.HasKey(e => e.attachment_id).HasName("dispute_attachments_pkey");

            entity.Property(e => e.attachment_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.attachment_role).HasColumnType("character varying");
            entity.Property(e => e.file_type).HasColumnType("character varying");
            entity.Property(e => e.file_url).HasColumnType("character varying");
            entity.Property(e => e.uploaded_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.dispute).WithMany(p => p.dispute_attachments)
                .HasForeignKey(d => d.dispute_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dispute_attachments_dispute_id_fkey");

            entity.HasOne(d => d.uploaded_byNavigation).WithMany(p => p.dispute_attachments)
                .HasForeignKey(d => d.uploaded_by)
                .HasConstraintName("dispute_attachments_uploaded_by_fkey");
        });

        modelBuilder.Entity<email_otp>(entity =>
        {
            entity.HasKey(e => e.otp_id).HasName("email_otps_pkey");

            entity.HasIndex(e => e.email, "idx_email_otps_email");

            entity.HasIndex(e => e.expire_at, "idx_email_otps_expire");

            entity.Property(e => e.otp_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.attempt_count).HasDefaultValue(0);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp with time zone");
            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property(e => e.expire_at).HasColumnType("timestamp with time zone");
            entity.Property(e => e.is_used).HasDefaultValue(false);
            entity.Property(e => e.otp_code).HasMaxLength(6);
            entity.Property(e => e.purpose)
                .HasColumnType("character varying")
                .HasConversion(
                    v => v.ToString().ToUpperInvariant(),
                    v => Enum.Parse<OTPPurpose>(v, true));
        });

        modelBuilder.Entity<facility>(entity =>
        {
            entity.HasKey(e => e.facility_id).HasName("facilities_pkey");

            entity.HasIndex(e => new { e.latitude, e.longitude }, "idx_facilities_location");

            entity.Property(e => e.facility_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.address_city).HasColumnType("character varying");
            entity.Property(e => e.address_district).HasColumnType("character varying");
            entity.Property(e => e.address_street).HasColumnType("character varying");
            entity.Property(e => e.address_ward).HasColumnType("character varying");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.facility_name).HasColumnType("character varying");
            entity.Property(e => e.facility_status)
                .HasDefaultValueSql("'PENDING'::character varying")
                .HasColumnType("character varying");
            entity.Property(e => e.latitude).HasPrecision(11, 9);
            entity.Property(e => e.longitude).HasPrecision(11, 9);

            entity.HasOne(d => d.brand).WithMany(p => p.facilities)
                .HasForeignKey(d => d.brand_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("facilities_brand_id_fkey");

            //entity.HasMany(d => d.amenities).WithMany(p => p.facilities)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "facility_amenity",
            //        r => r.HasOne<amenity>().WithMany()
            //            .HasForeignKey("amenity_id")
            //            .HasConstraintName("facility_amenities_amenity_id_fkey"),
            //        l => l.HasOne<facility>().WithMany()
            //            .HasForeignKey("facility_id")
            //            .HasConstraintName("facility_amenities_facility_id_fkey"),
            //        j =>
            //        {
            //            j.HasKey("facility_id", "amenity_id").HasName("facility_amenities_pkey");
            //            j.ToTable("facility_amenities");
            //        });
        });

        modelBuilder.Entity<facility_amenity>()
            .HasKey(x => new { x.facility_id, x.amenity_id });

        modelBuilder.Entity<facility_amenity>()
            .HasOne(fa => fa.facility)
            .WithMany(f => f.facility_amenities)
            .HasForeignKey(fa => fa.facility_id);

        modelBuilder.Entity<facility_amenity>()
            .HasOne(fa => fa.amenity)
            .WithMany(a => a.facility_amenities)
            .HasForeignKey(fa => fa.amenity_id);

        modelBuilder.Entity<facility_area>(entity =>
        {
            entity.HasKey(e => e.area_id).HasName("facility_area_pkey");

            entity.ToTable("facility_area");

            entity.HasIndex(e => e.facility_id, "idx_facility_area_facility");

            entity.HasIndex(e => new { e.facility_id, e.area_name }, "ux_facility_area_name").IsUnique();

            entity.Property(e => e.area_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.area_name).HasColumnType("character varying");
            entity.Property(e => e.is_active).HasDefaultValue(true);

            entity.HasOne(d => d.facility).WithMany(p => p.facility_areas)
                .HasForeignKey(d => d.facility_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("facility_area_facility_id_fkey");
        });

        modelBuilder.Entity<facility_document>(entity =>
        {
            entity.ToTable("facility_documents");

            entity.HasKey(e => e.document_id).HasName("facility_documents_pkey");
            entity.Property(e => e.document_id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasIndex(e => new { e.facility_id, e.document_status }, "idx_facility_docs_status");
            entity.HasIndex(e => new { e.facility_id, e.document_type, e.version }, "uq_facility_document_type").IsUnique();

            entity.Property(e => e.attachments).HasColumnType("jsonb");

            entity.Property(e => e.document_status)
                .HasDefaultValueSql("'PENDING'::character varying")
                .HasMaxLength(20);
            entity.Property(e => e.document_type).HasColumnType("character varying");
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.facility).WithMany(p => p.facility_documents)
                .HasForeignKey(d => d.facility_id)
                .HasConstraintName("fk_facility")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.reviewed_by_navigation).WithMany(p => p.facility_documents)
                .HasForeignKey(d => d.reviewed_by)
                .HasConstraintName("fk_reviewer");
        });

        modelBuilder.Entity<host_addon_price>(entity =>
        {
            entity.HasKey(e => e.host_service_id).HasName("host_addon_prices_pkey");

            entity.HasIndex(e => e.facility_id, "idx_host_addon_prices_facility");

            entity.HasIndex(e => new { e.facility_id, e.service_id }, "ux_host_addon_service").IsUnique();

            entity.Property(e => e.host_service_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.price).HasPrecision(12, 2);

            entity.HasOne(d => d.facility).WithMany(p => p.host_addon_prices)
                .HasForeignKey(d => d.facility_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("host_addon_prices_facility_id_fkey");

            entity.HasOne(d => d.service).WithMany(p => p.host_addon_prices)
                .HasForeignKey(d => d.service_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("host_addon_prices_service_id_fkey");
        });

        modelBuilder.Entity<host_base_price>(entity =>
        {
            entity.HasKey(e => e.host_price_id).HasName("host_base_prices_pkey");

            entity.HasIndex(e => e.facility_id, "idx_host_base_prices_facility");

            entity.HasIndex(e => new { e.facility_id, e.rule_id, e.box_type }, "ux_host_base_price").IsUnique();

            entity.Property(e => e.host_price_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.base_hour_price).HasPrecision(12, 2);
            entity.Property(e => e.base_overnight_price).HasPrecision(12, 2);
            entity.Property(e => e.box_type).HasColumnType("character varying");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_active).HasDefaultValue(true);

            entity.HasOne(d => d.facility).WithMany(p => p.host_base_prices)
                .HasForeignKey(d => d.facility_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("host_base_prices_facility_id_fkey");

            entity.HasOne(d => d.rule).WithMany(p => p.host_base_prices)
                .HasForeignKey(d => d.rule_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("host_base_prices_rule_id_fkey");
        });

        modelBuilder.Entity<host_document>(entity =>
        {
            entity.HasKey(e => e.document_id).HasName("host_documents_pkey");

            entity.HasIndex(e => new { e.host_id, e.document_type, e.version }, "ux_host_documents_version").IsUnique();

            entity.Property(e => e.document_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.attachments).HasColumnType("jsonb");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.document_status)
                .HasDefaultValueSql("'PENDING'::character varying")
                .HasColumnType("character varying");
            entity.Property(e => e.document_type).HasColumnType("character varying");

            entity.HasOne(d => d.host).WithMany(p => p.host_documents)
                .HasForeignKey(d => d.host_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_host_documents_host");

            entity.HasOne(d => d.reviewed_byNavigation).WithMany(p => p.host_documents)
                .HasForeignKey(d => d.reviewed_by)
                .HasConstraintName("fk_host_documents_reviewer");
        });

        modelBuilder.Entity<host_payout_account>(entity =>
        {
            entity.HasKey(e => e.account_id).HasName("host_payout_accounts_pkey");

            entity.HasIndex(e => e.host_id, "ux_host_primary_payout")
                .IsUnique()
                .HasFilter("(is_primary = true)");

            entity.Property(e => e.account_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.account_name).HasColumnType("character varying");
            entity.Property(e => e.account_number).HasColumnType("character varying");
            entity.Property(e => e.bank_name).HasColumnType("character varying");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_primary).HasDefaultValue(false);
            entity.Property(e => e.payment_method).HasColumnType("character varying");

            entity.HasOne(d => d.host).WithOne(p => p.host_payout_account)
                .HasForeignKey<host_payout_account>(d => d.host_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("host_payout_accounts_host_id_fkey");
        });

        modelBuilder.Entity<host_profile>(entity =>
        {
            entity.HasKey(e => e.host_id).HasName("host_profiles_pkey");

            entity.HasIndex(e => e.user_id, "host_profiles_user_id_key").IsUnique();
            entity.HasIndex(e => e.representative_id_number, "idx_host_profiles_representative_id_number").IsUnique();
            entity.Property(e => e.host_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.representative_id_name).HasColumnType("character varying");
            entity.Property(e => e.representative_id_number).HasColumnType("character varying");
            entity.Property(e => e.tax_code).HasColumnType("character varying");
            entity.Property(e => e.representative_front_url).HasColumnType("character varying");
            entity.Property(e => e.representative_back_url).HasColumnType("character varying");
            entity.Property(e => e.business_name).HasColumnType("character varying");
            entity.Property(e => e.address_district).HasColumnType("character varying");
            entity.Property(e => e.address_ward).HasColumnType("character varying");
            entity.Property(e => e.address_detail).HasColumnType("text");
            entity.Property(e => e.reject_reason).HasColumnType("text");
            entity.Property(e => e.verified_status)
                .HasDefaultValueSql("'PENDING'::character varying")
                .HasColumnType("character varying");

            entity.HasOne(d => d.user).WithOne(p => p.host_profileuser)
                .HasForeignKey<host_profile>(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_host_profiles_user");

            entity.HasOne(d => d.verified_byNavigation).WithMany(p => p.host_profileverified_byNavigations)
                .HasForeignKey(d => d.verified_by)
                .HasConstraintName("fk_host_profiles_verified_by");
        });

        modelBuilder.Entity<host_registration_draft>(entity =>
        {
            entity.HasKey(e => e.draft_id).HasName("host_registration_drafts_pkey");

            entity.HasIndex(e => e.email, "idx_draft_email");

            entity.HasIndex(e => e.expire_at, "idx_draft_expire");

            entity.Property(e => e.draft_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp with time zone");
            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property(e => e.expire_at).HasColumnType("timestamp with time zone");
            entity.Property(e => e.is_verified).HasDefaultValue(false);
            entity.Property(e => e.payload).HasColumnType("jsonb");
            entity.Property(e => e.phone).HasMaxLength(20);
            entity.Property(e => e.updated_at).HasColumnType("timestamp with time zone");

            entity.HasOne(d => d.otp).WithOne(p => p.draft)
                .HasForeignKey<host_registration_draft>(d => d.otp_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_draft_otp");
        });

        modelBuilder.Entity<media_asset>(entity =>
        {
            entity.HasKey(e => e.media_id).HasName("media_assets_pkey");

            entity.HasIndex(e => new { e.target_type, e.target_id }, "idx_media_assets_target");

            entity.HasIndex(e => new { e.target_type, e.target_id }, "ux_media_cover")
                .IsUnique()
                .HasFilter("(is_cover = true)");

            entity.Property(e => e.media_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.display_order).HasDefaultValue(0);
            entity.Property(e => e.is_cover).HasDefaultValue(false);
            entity.Property(e => e.media_type).HasColumnType("character varying");
            entity.Property(e => e.media_url).HasColumnType("character varying");
            entity.Property(e => e.target_type).HasColumnType("character varying");
            entity.Property(e => e.thumbnail_url).HasColumnType("character varying");

            entity.HasOne(d => d.uploaded_byNavigation).WithMany(p => p.media_assets)
                .HasForeignKey(d => d.uploaded_by)
                .HasConstraintName("media_assets_uploaded_by_fkey");
        });

        modelBuilder.Entity<message>(entity =>
        {
            entity.HasKey(e => e.message_id).HasName("messages_pkey");

            entity.HasIndex(e => new { e.conversation_id, e.sent_at }, "idx_messages_conversation_time");

            entity.HasIndex(e => e.sender_id, "idx_messages_sender");

            entity.Property(e => e.message_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.is_read).HasDefaultValue(false);
            entity.Property(e => e.message_type).HasColumnType("character varying");
            entity.Property(e => e.sent_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.conversation).WithMany(p => p.messages)
                .HasForeignKey(d => d.conversation_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("messages_conversation_id_fkey");

            entity.HasOne(d => d.sender).WithMany(p => p.messages)
                .HasForeignKey(d => d.sender_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("messages_sender_id_fkey");
        });

        modelBuilder.Entity<notification>(entity =>
        {
            entity.HasKey(e => e.notification_id).HasName("notifications_pkey");

            entity.HasIndex(e => new { e.user_id, e.created_at }, "idx_notifications_user_time").IsDescending(false, true);

            entity.Property(e => e.notification_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.notification_type).HasColumnType("character varying");
            entity.Property(e => e.target_type).HasColumnType("character varying");
            entity.Property(e => e.title).HasColumnType("character varying");

            entity.HasOne(d => d.user).WithMany(p => p.notifications)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("notifications_user_id_fkey");
        });

        modelBuilder.Entity<payment>(entity =>
        {
            entity.HasKey(e => e.payment_id).HasName("payments_pkey");

            entity.HasIndex(e => e.booking_id, "idx_payments_booking");

            entity.HasIndex(e => e.payment_status, "idx_payments_status");

            entity.HasIndex(e => e.client_request_id, "payments_client_request_id_key").IsUnique();

            entity.Property(e => e.payment_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.amount).HasPrecision(12, 2);
            entity.Property(e => e.client_request_id).HasColumnType("character varying");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.currency)
                .HasDefaultValueSql("'VND'::character varying")
                .HasColumnType("character varying");
            entity.Property(e => e.payment_status).HasColumnType("character varying");
            entity.Property(e => e.payment_type).HasColumnType("character varying");
            entity.Property(e => e.reference_note).HasColumnType("character varying");

            entity.HasOne(d => d.booking).WithMany(p => p.payments)
                .HasForeignKey(d => d.booking_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payments_booking_id_fkey");
        });

        modelBuilder.Entity<payment_transaction>(entity =>
        {
            entity.HasKey(e => e.transaction_id).HasName("payment_transactions_pkey");

            entity.HasIndex(e => e.gateway_transaction_id, "idx_payment_transactions_gateway");

            entity.HasIndex(e => e.payment_id, "idx_payment_transactions_payment");

            entity.Property(e => e.transaction_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.environment).HasColumnType("character varying");
            entity.Property(e => e.gateway_transaction_id).HasColumnType("character varying");
            entity.Property(e => e.provider).HasColumnType("character varying");
            entity.Property(e => e.provider_response_code).HasColumnType("character varying");
            entity.Property(e => e.request_type).HasColumnType("character varying");
            entity.Property(e => e.transaction_status).HasColumnType("character varying");

            entity.HasOne(d => d.payment).WithMany(p => p.payment_transactions)
                .HasForeignKey(d => d.payment_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payment_transactions_payment_id_fkey");
        });

        modelBuilder.Entity<platform_fee_config>(entity =>
        {
            entity.HasKey(e => e.config_id).HasName("platform_fee_configs_pkey");

            entity.HasIndex(e => new { e.fee_code, e.effective_from }, "idx_platform_fee_code_effective");

            entity.HasIndex(e => e.target_host_id, "idx_platform_fee_target_host");

            entity.HasIndex(e => new { e.fee_code, e.effective_from }, "ux_platform_fee_code_time").IsUnique();

            entity.Property(e => e.config_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.applied_base_on).HasColumnType("character varying");
            entity.Property(e => e.calculation_method).HasColumnType("character varying");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.fee_code).HasColumnType("character varying");
            entity.Property(e => e.fee_name).HasColumnType("character varying");
            entity.Property(e => e.fee_type).HasColumnType("character varying");
            entity.Property(e => e.fixed_amount).HasPrecision(12, 2);
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.percentage_value).HasPrecision(5, 2);
            entity.Property(e => e.priority).HasDefaultValue(0);

            entity.HasOne(d => d.target_host).WithMany(p => p.platform_fee_configs)
                .HasForeignKey(d => d.target_host_id)
                .HasConstraintName("platform_fee_configs_target_host_id_fkey");
        });

        modelBuilder.Entity<pricing_combo>(entity =>
        {
            entity.HasKey(e => e.combo_id).HasName("pricing_combos_pkey");

            entity.HasIndex(e => new { e.rule_id, e.hours }, "ux_pricing_combo_rule_hours").IsUnique();

            entity.Property(e => e.combo_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.combo_factor).HasPrecision(3, 2);
            entity.Property(e => e.is_active).HasDefaultValue(true);

            entity.HasOne(d => d.rule).WithMany(p => p.pricing_combos)
                .HasForeignKey(d => d.rule_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pricing_combos_rule_id_fkey");
        });

        modelBuilder.Entity<pricing_factor>(entity =>
        {
            entity.HasKey(e => e.factor_id).HasName("pricing_factors_pkey");

            entity.HasIndex(e => e.rule_id, "idx_pricing_factors_rule");

            entity.Property(e => e.factor_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.factor_type).HasColumnType("character varying");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.max_factor).HasPrecision(3, 2);
            entity.Property(e => e.min_factor).HasPrecision(3, 2);
            entity.Property(e => e.priority).HasDefaultValue(0);
            entity.Property(e => e.ref_code).HasColumnType("character varying");

            entity.HasOne(d => d.rule).WithMany(p => p.pricing_factors)
                .HasForeignKey(d => d.rule_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pricing_factors_rule_id_fkey");
        });

        modelBuilder.Entity<review>(entity =>
        {
            entity.HasKey(e => e.review_id).HasName("reviews_pkey");

            entity.HasIndex(e => e.facility_id, "idx_reviews_facility");

            entity.HasIndex(e => e.guest_id, "idx_reviews_guest");

            entity.HasIndex(e => e.booking_id, "reviews_booking_id_key").IsUnique();

            entity.Property(e => e.review_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.booking).WithOne(p => p.review)
                .HasForeignKey<review>(d => d.booking_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reviews_booking_id_fkey");

            entity.HasOne(d => d.facility).WithMany(p => p.reviews)
                .HasForeignKey(d => d.facility_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reviews_facility_id_fkey");

            entity.HasOne(d => d.guest).WithMany(p => p.reviews)
                .HasForeignKey(d => d.guest_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reviews_guest_id_fkey");
        });

        modelBuilder.Entity<sleepbox>(entity =>
        {
            entity.HasKey(e => e.box_id).HasName("sleepbox_pkey");

            entity.ToTable("sleepbox");

            entity.HasIndex(e => new { e.area_id, e.row_number, e.level_number }, "ux_sleepbox_position").IsUnique();

            entity.Property(e => e.box_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.box_class).HasColumnType("character varying");
            entity.Property(e => e.box_name).HasColumnType("character varying");
            entity.Property(e => e.box_status).HasColumnType("character varying");
            entity.Property(e => e.box_type).HasColumnType("character varying");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.area).WithMany(p => p.sleepboxes)
                .HasForeignKey(d => d.area_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sleepbox_area_id_fkey");

            //entity.HasMany(d => d.amenities).WithMany(p => p.boxes)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "sleepbox_amenity",
            //        r => r.HasOne<amenity>().WithMany()
            //            .HasForeignKey("amenity_id")
            //            .HasConstraintName("sleepbox_amenities_amenity_id_fkey"),
            //        l => l.HasOne<sleepbox>().WithMany()
            //            .HasForeignKey("box_id")
            //            .HasConstraintName("sleepbox_amenities_box_id_fkey"),
            //        j =>
            //        {
            //            j.HasKey("box_id", "amenity_id").HasName("sleepbox_amenities_pkey");
            //            j.ToTable("sleepbox_amenities");
            //        });
        });

        modelBuilder.Entity<sleepbox_amenity>()
            .HasKey(x => new { x.box_id, x.amenity_id });

        modelBuilder.Entity<sleepbox_amenity>()
            .HasOne(sa => sa.sleepbox)
            .WithMany(sb => sb.sleepbox_amenities)
            .HasForeignKey(sa => sa.box_id);

        modelBuilder.Entity<sleepbox_amenity>()
            .HasOne(sa => sa.amenity)
            .WithMany(a => a.sleepbox_amenities)
            .HasForeignKey(sa => sa.amenity_id);

        modelBuilder.Entity<staff_profile>(entity =>
        {
            entity.HasKey(e => e.staff_id).HasName("staff_profiles_pkey");

            entity.HasIndex(e => e.facility_id, "idx_staff_profiles_facility");

            entity.HasIndex(e => e.user_id, "idx_staff_profiles_user");

            entity.Property(e => e.staff_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.position).HasColumnType("character varying");

            entity.HasOne(d => d.facility).WithMany(p => p.staff_profiles)
                .HasForeignKey(d => d.facility_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_staff_profiles_facility");

            entity.HasOne(d => d.user).WithMany(p => p.staff_profiles)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_staff_profiles_user");
        });

        modelBuilder.Entity<system_policy>(entity =>
        {
            entity.HasKey(e => e.policy_id).HasName("system_policies_pkey");

            entity.HasIndex(e => e.policy_type, "idx_system_policies_type");

            entity.Property(e => e.policy_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.policy_type).HasColumnType("character varying");
            entity.Property(e => e.title).HasColumnType("character varying");
            entity.Property(e => e.version).HasColumnType("character varying");
        });

        modelBuilder.Entity<system_price_rule>(entity =>
        {
            entity.HasKey(e => e.rule_id).HasName("system_price_rules_pkey");

            entity.HasIndex(e => new { e.pricing_mode, e.is_active }, "idx_price_rules_mode_active");

            entity.Property(e => e.rule_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.max_price).HasPrecision(12, 2);
            entity.Property(e => e.min_price).HasPrecision(12, 2);
            entity.Property(e => e.pricing_mode).HasColumnType("character varying");
            entity.Property(e => e.priority).HasDefaultValue(0);
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.user_id).HasName("users_pkey");

            entity.HasIndex(e => e.email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.username, "users_username_key").IsUnique();

            entity.Property(e => e.user_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.email).HasColumnType("character varying");
            entity.Property(e => e.is_email_verified).HasDefaultValue(false);
            entity.Property(e => e.password_hash).HasColumnType("character varying");
            entity.Property(e => e.phone).HasColumnType("character varying");
            entity.Property(e => e.role)
                .HasColumnType("character varying")
                .HasConversion(
                    v => v.ToString().ToUpperInvariant(),
                    v => Enum.Parse<UserRole>(v, true));
            entity.Property(e => e.user_status)
                .HasColumnType("character varying")
                .HasDefaultValueSql("'ACTIVE'::character varying")
                .HasConversion(
                    v => v.ToString().ToUpperInvariant(),
                    v => Enum.Parse<UserStatus>(v, true));
            entity.Property(e => e.username).HasColumnType("character varying");
        });

        modelBuilder.Entity<user_favorite>(entity =>
        {
            entity.HasKey(e => e.favorite_id).HasName("user_favorites_pkey");

            entity.HasIndex(e => new { e.user_id, e.facility_id }, "ux_user_favorites").IsUnique();

            entity.Property(e => e.favorite_id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.facility).WithMany(p => p.user_favorites)
                .HasForeignKey(d => d.facility_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_favorites_facility_id_fkey");

            entity.HasOne(d => d.user).WithMany(p => p.user_favorites)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_favorites_user_id_fkey");
        });

        modelBuilder.Entity<user_profile>(entity =>
        {
            entity.HasKey(e => e.profile_id).HasName("user_profiles_pkey");

            entity.HasIndex(e => e.user_id, "user_profiles_user_id_key").IsUnique();

            entity.Property(e => e.profile_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.avatar_url).HasColumnType("character varying");
            entity.Property(e => e.first_name).HasColumnType("character varying");
            entity.Property(e => e.gender).HasColumnType("character varying");
            entity.Property(e => e.last_name).HasColumnType("character varying");
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.user).WithOne(p => p.user_profile)
                .HasForeignKey<user_profile>(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_profiles_user");
        });

        modelBuilder.Entity<wallet>(entity =>
        {
            entity.HasKey(e => e.wallet_id).HasName("wallets_pkey");

            entity.HasIndex(e => e.host_id, "idx_wallets_host");

            entity.HasIndex(e => e.wallet_type, "idx_wallets_type");

            entity.Property(e => e.wallet_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.available_balance)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("0");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.currency)
                .HasDefaultValueSql("'VND'::character varying")
                .HasColumnType("character varying");
            entity.Property(e => e.pending_balance)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("0");
            entity.Property(e => e.wallet_type).HasColumnType("character varying");

            entity.HasOne(d => d.host).WithMany(p => p.wallets)
                .HasForeignKey(d => d.host_id)
                .HasConstraintName("wallets_host_id_fkey");
        });

        modelBuilder.Entity<wallet_transaction>(entity =>
        {
            entity.HasKey(e => e.transaction_id).HasName("wallet_transactions_pkey");

            entity.HasIndex(e => new { e.reference_type, e.reference_id }, "idx_wallet_transactions_reference");

            entity.HasIndex(e => e.transaction_type, "idx_wallet_transactions_type");

            entity.HasIndex(e => new { e.wallet_id, e.created_at }, "idx_wallet_transactions_wallet_time").IsDescending(false, true);

            entity.Property(e => e.transaction_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.amount).HasPrecision(12, 2);
            entity.Property(e => e.available_balance_after).HasPrecision(12, 2);
            entity.Property(e => e.balance_type).HasColumnType("character varying");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.direction).HasColumnType("character varying");
            entity.Property(e => e.pending_balance_after).HasPrecision(12, 2);
            entity.Property(e => e.reference_type).HasColumnType("character varying");
            entity.Property(e => e.transaction_status)
                .HasDefaultValueSql("'SUCCESS'::character varying")
                .HasColumnType("character varying");
            entity.Property(e => e.transaction_type).HasColumnType("character varying");

            entity.HasOne(d => d.wallet).WithMany(p => p.wallet_transactions)
                .HasForeignKey(d => d.wallet_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("wallet_transactions_wallet_id_fkey");
        });

        modelBuilder.Entity<withdrawal_request>(entity =>
        {
            entity.HasKey(e => e.request_id).HasName("withdrawal_requests_pkey");

            entity.HasIndex(e => e.host_id, "idx_withdrawal_requests_host");

            entity.HasIndex(e => e.status, "idx_withdrawal_requests_status");

            entity.HasIndex(e => e.wallet_id, "idx_withdrawal_requests_wallet");

            entity.HasIndex(e => e.request_code, "withdrawal_requests_request_code_key").IsUnique();

            entity.Property(e => e.request_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.amount).HasPrecision(12, 2);
            entity.Property(e => e.bank_transaction_code).HasColumnType("character varying");
            entity.Property(e => e.request_code).HasColumnType("character varying");
            entity.Property(e => e.requested_at).HasDefaultValueSql("now()");
            entity.Property(e => e.status).HasColumnType("character varying");

            entity.HasOne(d => d.account).WithMany(p => p.withdrawal_requests)
                .HasForeignKey(d => d.account_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("withdrawal_requests_account_id_fkey");

            entity.HasOne(d => d.host).WithMany(p => p.withdrawal_requests)
                .HasForeignKey(d => d.host_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("withdrawal_requests_host_id_fkey");

            entity.HasOne(d => d.wallet).WithMany(p => p.withdrawal_requests)
                .HasForeignKey(d => d.wallet_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("withdrawal_requests_wallet_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
