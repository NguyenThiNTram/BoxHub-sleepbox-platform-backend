using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BoxHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorPricingBoxType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:auth.aal_level", "aal1,aal2,aal3")
                .Annotation("Npgsql:Enum:auth.code_challenge_method", "s256,plain")
                .Annotation("Npgsql:Enum:auth.factor_status", "unverified,verified")
                .Annotation("Npgsql:Enum:auth.factor_type", "totp,webauthn,phone")
                .Annotation("Npgsql:Enum:auth.oauth_authorization_status", "pending,approved,denied,expired")
                .Annotation("Npgsql:Enum:auth.oauth_client_type", "public,confidential")
                .Annotation("Npgsql:Enum:auth.oauth_registration_type", "dynamic,manual")
                .Annotation("Npgsql:Enum:auth.oauth_response_type", "code")
                .Annotation("Npgsql:Enum:auth.one_time_token_type", "confirmation_token,reauthentication_token,recovery_token,email_change_token_new,email_change_token_current,phone_change_token")
                .Annotation("Npgsql:Enum:realtime.action", "INSERT,UPDATE,DELETE,TRUNCATE,ERROR")
                .Annotation("Npgsql:Enum:realtime.equality_op", "eq,neq,lt,lte,gt,gte,in")
                .Annotation("Npgsql:Enum:storage.buckettype", "STANDARD,ANALYTICS,VECTOR")
                .Annotation("Npgsql:PostgresExtension:btree_gist", ",,")
                .Annotation("Npgsql:PostgresExtension:extensions.pg_stat_statements", ",,")
                .Annotation("Npgsql:PostgresExtension:extensions.pgcrypto", ",,")
                .Annotation("Npgsql:PostgresExtension:extensions.uuid-ossp", ",,")
                .Annotation("Npgsql:PostgresExtension:graphql.pg_graphql", ",,")
                .Annotation("Npgsql:PostgresExtension:vault.supabase_vault", ",,");

            migrationBuilder.CreateTable(
                name: "addon_services",
                columns: table => new
                {
                    service_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    service_name = table.Column<string>(type: "character varying", nullable: false),
                    unit = table.Column<string>(type: "character varying", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("addon_services_pkey", x => x.service_id);
                });

            migrationBuilder.CreateTable(
                name: "amenities",
                columns: table => new
                {
                    amenity_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    amenity_name = table.Column<string>(type: "character varying", nullable: false),
                    amenity_type = table.Column<string>(type: "character varying", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("amenities_pkey", x => x.amenity_id);
                });

            migrationBuilder.CreateTable(
                name: "email_otps",
                columns: table => new
                {
                    otp_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    otp_code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    purpose = table.Column<string>(type: "character varying", nullable: false),
                    expire_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    attempt_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("email_otps_pkey", x => x.otp_id);
                });

            migrationBuilder.CreateTable(
                name: "system_box_type_price_limits",
                columns: table => new
                {
                    limit_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    capacity_type = table.Column<string>(type: "character varying", nullable: false),
                    box_class = table.Column<string>(type: "character varying", nullable: false),
                    min_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    max_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("system_box_type_price_limits_pkey", x => x.limit_id);
                    table.CheckConstraint("ck_price_range", "max_price > min_price");
                });

            migrationBuilder.CreateTable(
                name: "system_policies",
                columns: table => new
                {
                    policy_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    policy_type = table.Column<string>(type: "character varying", nullable: false),
                    title = table.Column<string>(type: "character varying", nullable: false),
                    content = table.Column<string>(type: "text", nullable: true),
                    version = table.Column<string>(type: "character varying", nullable: true),
                    effective_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("system_policies_pkey", x => x.policy_id);
                });

            migrationBuilder.CreateTable(
                name: "system_price_rules",
                columns: table => new
                {
                    rule_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    priority = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    pricing_mode = table.Column<string>(type: "character varying", nullable: false),
                    min_hours = table.Column<int>(type: "integer", nullable: true),
                    max_hours = table.Column<int>(type: "integer", nullable: true),
                    fixed_start_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    fixed_end_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    min_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    max_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("system_price_rules_pkey", x => x.rule_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    username = table.Column<string>(type: "character varying", nullable: false),
                    email = table.Column<string>(type: "character varying", nullable: false),
                    phone = table.Column<string>(type: "character varying", nullable: true),
                    password_hash = table.Column<string>(type: "character varying", nullable: false),
                    role = table.Column<string>(type: "character varying", nullable: false),
                    user_status = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "'ACTIVE'::character varying"),
                    is_email_verified = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    email_verified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_login_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "host_registration_drafts",
                columns: table => new
                {
                    draft_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    otp_id = table.Column<Guid>(type: "uuid", nullable: true),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    expire_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("host_registration_drafts_pkey", x => x.draft_id);
                    table.ForeignKey(
                        name: "fk_draft_otp",
                        column: x => x.otp_id,
                        principalTable: "email_otps",
                        principalColumn: "otp_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "pricing_combos",
                columns: table => new
                {
                    combo_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    rule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    hours = table.Column<int>(type: "integer", nullable: false),
                    combo_factor = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pricing_combos_pkey", x => x.combo_id);
                    table.ForeignKey(
                        name: "pricing_combos_rule_id_fkey",
                        column: x => x.rule_id,
                        principalTable: "system_price_rules",
                        principalColumn: "rule_id");
                });

            migrationBuilder.CreateTable(
                name: "pricing_factors",
                columns: table => new
                {
                    factor_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    rule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    factor_type = table.Column<string>(type: "character varying", nullable: false),
                    ref_code = table.Column<string>(type: "character varying", nullable: true),
                    start_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    end_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    min_factor = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: true),
                    max_factor = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: true),
                    priority = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pricing_factors_pkey", x => x.factor_id);
                    table.ForeignKey(
                        name: "pricing_factors_rule_id_fkey",
                        column: x => x.rule_id,
                        principalTable: "system_price_rules",
                        principalColumn: "rule_id");
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    audit_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    actor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    actor_role = table.Column<string>(type: "character varying", nullable: true),
                    action = table.Column<string>(type: "character varying", nullable: false),
                    target_type = table.Column<string>(type: "character varying", nullable: true),
                    target_id = table.Column<Guid>(type: "uuid", nullable: true),
                    old_value = table.Column<string>(type: "text", nullable: true),
                    new_value = table.Column<string>(type: "text", nullable: true),
                    note = table.Column<string>(type: "character varying", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("audit_logs_pkey", x => x.audit_id);
                    table.ForeignKey(
                        name: "audit_logs_actor_id_fkey",
                        column: x => x.actor_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "host_profiles",
                columns: table => new
                {
                    host_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    representative_name = table.Column<string>(type: "character varying", nullable: true),
                    representative_id_number = table.Column<string>(type: "character varying", nullable: true),
                    tax_code = table.Column<string>(type: "character varying", nullable: true),
                    business_address = table.Column<string>(type: "character varying", nullable: true),
                    verified_status = table.Column<string>(type: "character varying", nullable: true, defaultValueSql: "'PENDING'::character varying"),
                    verified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    verified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    verified_note = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("host_profiles_pkey", x => x.host_id);
                    table.ForeignKey(
                        name: "fk_host_profiles_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "fk_host_profiles_verified_by",
                        column: x => x.verified_by,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "media_assets",
                columns: table => new
                {
                    media_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    target_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_type = table.Column<string>(type: "character varying", nullable: false),
                    media_type = table.Column<string>(type: "character varying", nullable: false),
                    media_url = table.Column<string>(type: "character varying", nullable: false),
                    thumbnail_url = table.Column<string>(type: "character varying", nullable: true),
                    uploaded_by = table.Column<Guid>(type: "uuid", nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    is_cover = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("media_assets_pkey", x => x.media_id);
                    table.ForeignKey(
                        name: "media_assets_uploaded_by_fkey",
                        column: x => x.uploaded_by,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    notification_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notification_type = table.Column<string>(type: "character varying", nullable: false),
                    target_type = table.Column<string>(type: "character varying", nullable: true),
                    target_id = table.Column<Guid>(type: "uuid", nullable: true),
                    title = table.Column<string>(type: "character varying", nullable: true),
                    content = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("notifications_pkey", x => x.notification_id);
                    table.ForeignKey(
                        name: "notifications_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "user_profiles",
                columns: table => new
                {
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying", nullable: true),
                    last_name = table.Column<string>(type: "character varying", nullable: true),
                    gender = table.Column<string>(type: "character varying", nullable: true),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: true),
                    avatar_url = table.Column<string>(type: "character varying", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_profiles_pkey", x => x.profile_id);
                    table.ForeignKey(
                        name: "fk_user_profiles_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "brands",
                columns: table => new
                {
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    host_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_name = table.Column<string>(type: "character varying", nullable: false),
                    brand_avatar = table.Column<string>(type: "character varying", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("brands_pkey", x => x.brand_id);
                    table.ForeignKey(
                        name: "brands_host_id_fkey",
                        column: x => x.host_id,
                        principalTable: "host_profiles",
                        principalColumn: "host_id");
                });

            migrationBuilder.CreateTable(
                name: "host_documents",
                columns: table => new
                {
                    document_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    host_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_type = table.Column<string>(type: "character varying", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    attachments = table.Column<string>(type: "jsonb", nullable: false),
                    expiry_date = table.Column<DateOnly>(type: "date", nullable: true),
                    document_status = table.Column<string>(type: "character varying", nullable: true, defaultValueSql: "'PENDING'::character varying"),
                    reviewed_by = table.Column<Guid>(type: "uuid", nullable: true),
                    reviewed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    reject_reason = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("host_documents_pkey", x => x.document_id);
                    table.ForeignKey(
                        name: "fk_host_documents_host",
                        column: x => x.host_id,
                        principalTable: "host_profiles",
                        principalColumn: "host_id");
                    table.ForeignKey(
                        name: "fk_host_documents_reviewer",
                        column: x => x.reviewed_by,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "host_payout_accounts",
                columns: table => new
                {
                    account_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    host_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_method = table.Column<string>(type: "character varying", nullable: false),
                    account_name = table.Column<string>(type: "character varying", nullable: true),
                    account_number = table.Column<string>(type: "character varying", nullable: false),
                    bank_name = table.Column<string>(type: "character varying", nullable: true),
                    is_primary = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("host_payout_accounts_pkey", x => x.account_id);
                    table.ForeignKey(
                        name: "host_payout_accounts_host_id_fkey",
                        column: x => x.host_id,
                        principalTable: "host_profiles",
                        principalColumn: "host_id");
                });

            migrationBuilder.CreateTable(
                name: "platform_fee_configs",
                columns: table => new
                {
                    config_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    fee_code = table.Column<string>(type: "character varying", nullable: false),
                    fee_name = table.Column<string>(type: "character varying", nullable: false),
                    fee_type = table.Column<string>(type: "character varying", nullable: true),
                    target_host_id = table.Column<Guid>(type: "uuid", nullable: true),
                    calculation_method = table.Column<string>(type: "character varying", nullable: true),
                    percentage_value = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    fixed_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    applied_base_on = table.Column<string>(type: "character varying", nullable: true),
                    priority = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    calculation_order = table.Column<int>(type: "integer", nullable: true),
                    effective_from = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    effective_to = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("platform_fee_configs_pkey", x => x.config_id);
                    table.ForeignKey(
                        name: "platform_fee_configs_target_host_id_fkey",
                        column: x => x.target_host_id,
                        principalTable: "host_profiles",
                        principalColumn: "host_id");
                });

            migrationBuilder.CreateTable(
                name: "wallets",
                columns: table => new
                {
                    wallet_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    wallet_type = table.Column<string>(type: "character varying", nullable: false),
                    host_id = table.Column<Guid>(type: "uuid", nullable: true),
                    currency = table.Column<string>(type: "character varying", nullable: true, defaultValueSql: "'VND'::character varying"),
                    available_balance = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "0"),
                    pending_balance = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "0"),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("wallets_pkey", x => x.wallet_id);
                    table.ForeignKey(
                        name: "wallets_host_id_fkey",
                        column: x => x.host_id,
                        principalTable: "host_profiles",
                        principalColumn: "host_id");
                });

            migrationBuilder.CreateTable(
                name: "facilities",
                columns: table => new
                {
                    facility_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    facility_name = table.Column<string>(type: "character varying", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    address_street = table.Column<string>(type: "character varying", nullable: true),
                    address_ward = table.Column<string>(type: "character varying", nullable: true),
                    address_district = table.Column<string>(type: "character varying", nullable: true),
                    address_city = table.Column<string>(type: "character varying", nullable: true),
                    latitude = table.Column<decimal>(type: "numeric(11,9)", precision: 11, scale: 9, nullable: true),
                    longitude = table.Column<decimal>(type: "numeric(11,9)", precision: 11, scale: 9, nullable: true),
                    house_rules = table.Column<string>(type: "text", nullable: true),
                    cleaning_buffer_minutes = table.Column<int>(type: "integer", nullable: true),
                    facility_status = table.Column<string>(type: "character varying", nullable: true, defaultValueSql: "'PENDING'::character varying"),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("facilities_pkey", x => x.facility_id);
                    table.ForeignKey(
                        name: "facilities_brand_id_fkey",
                        column: x => x.brand_id,
                        principalTable: "brands",
                        principalColumn: "brand_id");
                });

            migrationBuilder.CreateTable(
                name: "wallet_transactions",
                columns: table => new
                {
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    wallet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_type = table.Column<string>(type: "character varying", nullable: false),
                    balance_type = table.Column<string>(type: "character varying", nullable: true),
                    direction = table.Column<string>(type: "character varying", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    reference_type = table.Column<string>(type: "character varying", nullable: true),
                    reference_id = table.Column<Guid>(type: "uuid", nullable: true),
                    available_balance_after = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    pending_balance_after = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    transaction_status = table.Column<string>(type: "character varying", nullable: true, defaultValueSql: "'SUCCESS'::character varying"),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("wallet_transactions_pkey", x => x.transaction_id);
                    table.ForeignKey(
                        name: "wallet_transactions_wallet_id_fkey",
                        column: x => x.wallet_id,
                        principalTable: "wallets",
                        principalColumn: "wallet_id");
                });

            migrationBuilder.CreateTable(
                name: "withdrawal_requests",
                columns: table => new
                {
                    request_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    request_code = table.Column<string>(type: "character varying", nullable: true),
                    host_id = table.Column<Guid>(type: "uuid", nullable: false),
                    wallet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    status = table.Column<string>(type: "character varying", nullable: true),
                    bank_transaction_code = table.Column<string>(type: "character varying", nullable: true),
                    admin_note = table.Column<string>(type: "text", nullable: true),
                    requested_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    processed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("withdrawal_requests_pkey", x => x.request_id);
                    table.ForeignKey(
                        name: "withdrawal_requests_account_id_fkey",
                        column: x => x.account_id,
                        principalTable: "host_payout_accounts",
                        principalColumn: "account_id");
                    table.ForeignKey(
                        name: "withdrawal_requests_host_id_fkey",
                        column: x => x.host_id,
                        principalTable: "host_profiles",
                        principalColumn: "host_id");
                    table.ForeignKey(
                        name: "withdrawal_requests_wallet_id_fkey",
                        column: x => x.wallet_id,
                        principalTable: "wallets",
                        principalColumn: "wallet_id");
                });

            migrationBuilder.CreateTable(
                name: "bookings",
                columns: table => new
                {
                    booking_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    booking_code = table.Column<string>(type: "character varying", nullable: true),
                    guest_id = table.Column<Guid>(type: "uuid", nullable: true),
                    facility_id = table.Column<Guid>(type: "uuid", nullable: true),
                    host_id = table.Column<Guid>(type: "uuid", nullable: true),
                    check_in = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    check_out = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    actual_check_in = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    actual_check_out = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    pricing_snapshot = table.Column<string>(type: "jsonb", nullable: true),
                    total_box_price = table.Column<decimal>(type: "numeric", nullable: true),
                    total_addon_item_price = table.Column<decimal>(type: "numeric", nullable: true),
                    commission_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    service_fee = table.Column<decimal>(type: "numeric", nullable: true),
                    vat_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    final_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    booking_status = table.Column<string>(type: "character varying", nullable: true),
                    payment_status = table.Column<string>(type: "character varying", nullable: true),
                    cancelled_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    cancelled_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("bookings_pkey", x => x.booking_id);
                    table.ForeignKey(
                        name: "bookings_cancelled_by_id_fkey",
                        column: x => x.cancelled_by_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "bookings_facility_id_fkey",
                        column: x => x.facility_id,
                        principalTable: "facilities",
                        principalColumn: "facility_id");
                    table.ForeignKey(
                        name: "bookings_guest_id_fkey",
                        column: x => x.guest_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "bookings_host_id_fkey",
                        column: x => x.host_id,
                        principalTable: "host_profiles",
                        principalColumn: "host_id");
                });

            migrationBuilder.CreateTable(
                name: "facility_amenities",
                columns: table => new
                {
                    facility_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amenity_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facility_amenities", x => new { x.facility_id, x.amenity_id });
                    table.ForeignKey(
                        name: "FK_facility_amenities_amenities_amenity_id",
                        column: x => x.amenity_id,
                        principalTable: "amenities",
                        principalColumn: "amenity_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_facility_amenities_facilities_facility_id",
                        column: x => x.facility_id,
                        principalTable: "facilities",
                        principalColumn: "facility_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility_area",
                columns: table => new
                {
                    area_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    facility_id = table.Column<Guid>(type: "uuid", nullable: false),
                    area_name = table.Column<string>(type: "character varying", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("facility_area_pkey", x => x.area_id);
                    table.ForeignKey(
                        name: "facility_area_facility_id_fkey",
                        column: x => x.facility_id,
                        principalTable: "facilities",
                        principalColumn: "facility_id");
                });

            migrationBuilder.CreateTable(
                name: "host_addon_prices",
                columns: table => new
                {
                    host_service_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    facility_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("host_addon_prices_pkey", x => x.host_service_id);
                    table.ForeignKey(
                        name: "host_addon_prices_facility_id_fkey",
                        column: x => x.facility_id,
                        principalTable: "facilities",
                        principalColumn: "facility_id");
                    table.ForeignKey(
                        name: "host_addon_prices_service_id_fkey",
                        column: x => x.service_id,
                        principalTable: "addon_services",
                        principalColumn: "service_id");
                });

            migrationBuilder.CreateTable(
                name: "host_base_prices",
                columns: table => new
                {
                    host_price_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    rule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    facility_id = table.Column<Guid>(type: "uuid", nullable: false),
                    capacity_type = table.Column<string>(type: "character varying", nullable: false),
                    box_class = table.Column<string>(type: "character varying", nullable: false),
                    base_hour_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    base_overnight_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("host_base_prices_pkey", x => x.host_price_id);
                    table.ForeignKey(
                        name: "host_base_prices_facility_id_fkey",
                        column: x => x.facility_id,
                        principalTable: "facilities",
                        principalColumn: "facility_id");
                    table.ForeignKey(
                        name: "host_base_prices_rule_id_fkey",
                        column: x => x.rule_id,
                        principalTable: "system_price_rules",
                        principalColumn: "rule_id");
                });

            migrationBuilder.CreateTable(
                name: "staff_profiles",
                columns: table => new
                {
                    staff_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    facility_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position = table.Column<string>(type: "character varying", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("staff_profiles_pkey", x => x.staff_id);
                    table.ForeignKey(
                        name: "fk_staff_profiles_facility",
                        column: x => x.facility_id,
                        principalTable: "facilities",
                        principalColumn: "facility_id");
                    table.ForeignKey(
                        name: "fk_staff_profiles_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "user_favorites",
                columns: table => new
                {
                    favorite_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    facility_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_favorites_pkey", x => x.favorite_id);
                    table.ForeignKey(
                        name: "user_favorites_facility_id_fkey",
                        column: x => x.facility_id,
                        principalTable: "facilities",
                        principalColumn: "facility_id");
                    table.ForeignKey(
                        name: "user_favorites_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "booking_addon_services",
                columns: table => new
                {
                    booking_addon_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: true),
                    service_name = table.Column<string>(type: "character varying", nullable: true),
                    unit_price = table.Column<decimal>(type: "numeric", nullable: true),
                    quantity = table.Column<decimal>(type: "numeric", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("booking_addon_services_pkey", x => x.booking_addon_id);
                    table.ForeignKey(
                        name: "booking_addon_services_booking_id_fkey",
                        column: x => x.booking_id,
                        principalTable: "bookings",
                        principalColumn: "booking_id");
                    table.ForeignKey(
                        name: "booking_addon_services_service_id_fkey",
                        column: x => x.service_id,
                        principalTable: "addon_services",
                        principalColumn: "service_id");
                });

            migrationBuilder.CreateTable(
                name: "disputes",
                columns: table => new
                {
                    dispute_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    raised_by = table.Column<Guid>(type: "uuid", nullable: false),
                    dispute_type = table.Column<string>(type: "character varying", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying", nullable: true, defaultValueSql: "'PENDING'::character varying"),
                    assigned_moderator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    moderator_note = table.Column<string>(type: "text", nullable: true),
                    resolution_type = table.Column<string>(type: "character varying", nullable: true),
                    refund_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "0"),
                    admin_approval_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    resolved_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("disputes_pkey", x => x.dispute_id);
                    table.ForeignKey(
                        name: "disputes_admin_approval_id_fkey",
                        column: x => x.admin_approval_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "disputes_assigned_moderator_id_fkey",
                        column: x => x.assigned_moderator_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "disputes_booking_id_fkey",
                        column: x => x.booking_id,
                        principalTable: "bookings",
                        principalColumn: "booking_id");
                    table.ForeignKey(
                        name: "disputes_raised_by_fkey",
                        column: x => x.raised_by,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_request_id = table.Column<string>(type: "character varying", nullable: true),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying", nullable: true, defaultValueSql: "'VND'::character varying"),
                    payment_type = table.Column<string>(type: "character varying", nullable: true),
                    payment_status = table.Column<string>(type: "character varying", nullable: true),
                    reference_note = table.Column<string>(type: "character varying", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("payments_pkey", x => x.payment_id);
                    table.ForeignKey(
                        name: "payments_booking_id_fkey",
                        column: x => x.booking_id,
                        principalTable: "bookings",
                        principalColumn: "booking_id");
                });

            migrationBuilder.CreateTable(
                name: "reviews",
                columns: table => new
                {
                    review_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    guest_id = table.Column<Guid>(type: "uuid", nullable: false),
                    facility_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating_score = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("reviews_pkey", x => x.review_id);
                    table.ForeignKey(
                        name: "reviews_booking_id_fkey",
                        column: x => x.booking_id,
                        principalTable: "bookings",
                        principalColumn: "booking_id");
                    table.ForeignKey(
                        name: "reviews_facility_id_fkey",
                        column: x => x.facility_id,
                        principalTable: "facilities",
                        principalColumn: "facility_id");
                    table.ForeignKey(
                        name: "reviews_guest_id_fkey",
                        column: x => x.guest_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "sleepbox",
                columns: table => new
                {
                    box_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    area_id = table.Column<Guid>(type: "uuid", nullable: false),
                    box_name = table.Column<string>(type: "character varying", nullable: false),
                    box_type = table.Column<string>(type: "character varying", nullable: true),
                    box_class = table.Column<string>(type: "character varying", nullable: true),
                    row_number = table.Column<int>(type: "integer", nullable: true),
                    level_number = table.Column<int>(type: "integer", nullable: true),
                    size_width = table.Column<decimal>(type: "numeric", nullable: true),
                    size_length = table.Column<decimal>(type: "numeric", nullable: true),
                    size_height = table.Column<decimal>(type: "numeric", nullable: true),
                    box_status = table.Column<string>(type: "character varying", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sleepbox_pkey", x => x.box_id);
                    table.ForeignKey(
                        name: "sleepbox_area_id_fkey",
                        column: x => x.area_id,
                        principalTable: "facility_area",
                        principalColumn: "area_id");
                });

            migrationBuilder.CreateTable(
                name: "conversations",
                columns: table => new
                {
                    conversation_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    conversation_type = table.Column<string>(type: "character varying", nullable: false),
                    guest_id = table.Column<Guid>(type: "uuid", nullable: true),
                    host_id = table.Column<Guid>(type: "uuid", nullable: true),
                    staff_id = table.Column<Guid>(type: "uuid", nullable: true),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: true),
                    facility_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    last_message_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("conversations_pkey", x => x.conversation_id);
                    table.ForeignKey(
                        name: "conversations_booking_id_fkey",
                        column: x => x.booking_id,
                        principalTable: "bookings",
                        principalColumn: "booking_id");
                    table.ForeignKey(
                        name: "conversations_facility_id_fkey",
                        column: x => x.facility_id,
                        principalTable: "facilities",
                        principalColumn: "facility_id");
                    table.ForeignKey(
                        name: "conversations_guest_id_fkey",
                        column: x => x.guest_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "conversations_host_id_fkey",
                        column: x => x.host_id,
                        principalTable: "host_profiles",
                        principalColumn: "host_id");
                    table.ForeignKey(
                        name: "conversations_staff_id_fkey",
                        column: x => x.staff_id,
                        principalTable: "staff_profiles",
                        principalColumn: "staff_id");
                });

            migrationBuilder.CreateTable(
                name: "dispute_attachments",
                columns: table => new
                {
                    attachment_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    dispute_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attachment_role = table.Column<string>(type: "character varying", nullable: true),
                    file_url = table.Column<string>(type: "character varying", nullable: false),
                    file_type = table.Column<string>(type: "character varying", nullable: true),
                    uploaded_by = table.Column<Guid>(type: "uuid", nullable: true),
                    uploaded_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("dispute_attachments_pkey", x => x.attachment_id);
                    table.ForeignKey(
                        name: "dispute_attachments_dispute_id_fkey",
                        column: x => x.dispute_id,
                        principalTable: "disputes",
                        principalColumn: "dispute_id");
                    table.ForeignKey(
                        name: "dispute_attachments_uploaded_by_fkey",
                        column: x => x.uploaded_by,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "booking_status_history",
                columns: table => new
                {
                    history_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    old_status = table.Column<string>(type: "character varying", nullable: true),
                    new_status = table.Column<string>(type: "character varying", nullable: true),
                    old_check_in = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    old_check_out = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    new_check_in = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    new_check_out = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    note = table.Column<string>(type: "character varying", nullable: true),
                    changed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("booking_status_history_pkey", x => x.history_id);
                    table.ForeignKey(
                        name: "booking_status_history_booking_id_fkey",
                        column: x => x.booking_id,
                        principalTable: "bookings",
                        principalColumn: "booking_id");
                    table.ForeignKey(
                        name: "booking_status_history_payment_id_fkey",
                        column: x => x.payment_id,
                        principalTable: "payments",
                        principalColumn: "payment_id");
                });

            migrationBuilder.CreateTable(
                name: "payment_transactions",
                columns: table => new
                {
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider = table.Column<string>(type: "character varying", nullable: false),
                    environment = table.Column<string>(type: "character varying", nullable: true),
                    gateway_transaction_id = table.Column<string>(type: "character varying", nullable: true),
                    request_type = table.Column<string>(type: "character varying", nullable: true),
                    transaction_status = table.Column<string>(type: "character varying", nullable: true),
                    provider_response_code = table.Column<string>(type: "character varying", nullable: true),
                    callback_payload = table.Column<string>(type: "text", nullable: true),
                    callback_received_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("payment_transactions_pkey", x => x.transaction_id);
                    table.ForeignKey(
                        name: "payment_transactions_payment_id_fkey",
                        column: x => x.payment_id,
                        principalTable: "payments",
                        principalColumn: "payment_id");
                });

            migrationBuilder.CreateTable(
                name: "booking_boxes",
                columns: table => new
                {
                    booking_box_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    box_id = table.Column<Guid>(type: "uuid", nullable: false),
                    box_price = table.Column<decimal>(type: "numeric", nullable: true),
                    box_name_snapshot = table.Column<string>(type: "character varying", nullable: true),
                    box_type_snapshot = table.Column<string>(type: "character varying", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("booking_boxes_pkey", x => x.booking_box_id);
                    table.ForeignKey(
                        name: "booking_boxes_booking_id_fkey",
                        column: x => x.booking_id,
                        principalTable: "bookings",
                        principalColumn: "booking_id");
                    table.ForeignKey(
                        name: "booking_boxes_box_id_fkey",
                        column: x => x.box_id,
                        principalTable: "sleepbox",
                        principalColumn: "box_id");
                });

            migrationBuilder.CreateTable(
                name: "box_availability",
                columns: table => new
                {
                    availability_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    facility_id = table.Column<Guid>(type: "uuid", nullable: false),
                    box_id = table.Column<Guid>(type: "uuid", nullable: false),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: true),
                    start_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    availability_status = table.Column<string>(type: "character varying", nullable: true),
                    locked_until = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("box_availability_pkey", x => x.availability_id);
                    table.ForeignKey(
                        name: "fk_box_availability_booking",
                        column: x => x.booking_id,
                        principalTable: "bookings",
                        principalColumn: "booking_id");
                    table.ForeignKey(
                        name: "fk_box_availability_box",
                        column: x => x.box_id,
                        principalTable: "sleepbox",
                        principalColumn: "box_id");
                    table.ForeignKey(
                        name: "fk_box_availability_facility",
                        column: x => x.facility_id,
                        principalTable: "facilities",
                        principalColumn: "facility_id");
                });

            migrationBuilder.CreateTable(
                name: "sleepbox_amenities",
                columns: table => new
                {
                    box_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amenity_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sleepbox_amenities", x => new { x.box_id, x.amenity_id });
                    table.ForeignKey(
                        name: "FK_sleepbox_amenities_amenities_amenity_id",
                        column: x => x.amenity_id,
                        principalTable: "amenities",
                        principalColumn: "amenity_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sleepbox_amenities_sleepbox_box_id",
                        column: x => x.box_id,
                        principalTable: "sleepbox",
                        principalColumn: "box_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    message_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    conversation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    message_type = table.Column<string>(type: "character varying", nullable: true),
                    content = table.Column<string>(type: "text", nullable: true),
                    sent_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    is_read = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("messages_pkey", x => x.message_id);
                    table.ForeignKey(
                        name: "messages_conversation_id_fkey",
                        column: x => x.conversation_id,
                        principalTable: "conversations",
                        principalColumn: "conversation_id");
                    table.ForeignKey(
                        name: "messages_sender_id_fkey",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "amenities_amenity_name_key",
                table: "amenities",
                column: "amenity_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_audit_logs_target",
                table: "audit_logs",
                columns: new[] { "target_type", "target_id" });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_actor_id",
                table: "audit_logs",
                column: "actor_id");

            migrationBuilder.CreateIndex(
                name: "idx_booking_addons_booking",
                table: "booking_addon_services",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_booking_addon_services_service_id",
                table: "booking_addon_services",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "idx_booking_boxes_booking",
                table: "booking_boxes",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "idx_booking_boxes_box",
                table: "booking_boxes",
                column: "box_id");

            migrationBuilder.CreateIndex(
                name: "idx_booking_status_history_booking",
                table: "booking_status_history",
                columns: new[] { "booking_id", "changed_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_booking_status_history_payment_id",
                table: "booking_status_history",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "bookings_booking_code_key",
                table: "bookings",
                column: "booking_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_bookings_check_in",
                table: "bookings",
                column: "check_in");

            migrationBuilder.CreateIndex(
                name: "idx_bookings_check_out",
                table: "bookings",
                column: "check_out");

            migrationBuilder.CreateIndex(
                name: "idx_bookings_facility_check_in",
                table: "bookings",
                columns: new[] { "facility_id", "check_in" });

            migrationBuilder.CreateIndex(
                name: "idx_bookings_guest_created",
                table: "bookings",
                columns: new[] { "guest_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "idx_bookings_host_created",
                table: "bookings",
                columns: new[] { "host_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "idx_bookings_status",
                table: "bookings",
                column: "booking_status");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_cancelled_by_id",
                table: "bookings",
                column: "cancelled_by_id");

            migrationBuilder.CreateIndex(
                name: "idx_box_availability_facility_time",
                table: "box_availability",
                columns: new[] { "facility_id", "start_time", "end_time" });

            migrationBuilder.CreateIndex(
                name: "idx_box_availability_status",
                table: "box_availability",
                columns: new[] { "availability_status", "locked_until" });

            migrationBuilder.CreateIndex(
                name: "IX_box_availability_booking_id",
                table: "box_availability",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_box_availability_box_id",
                table: "box_availability",
                column: "box_id");

            migrationBuilder.CreateIndex(
                name: "idx_brands_host",
                table: "brands",
                column: "host_id");

            migrationBuilder.CreateIndex(
                name: "ux_host_brand_name",
                table: "brands",
                columns: new[] { "host_id", "brand_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_conversations_guest",
                table: "conversations",
                column: "guest_id");

            migrationBuilder.CreateIndex(
                name: "idx_conversations_host",
                table: "conversations",
                column: "host_id");

            migrationBuilder.CreateIndex(
                name: "idx_conversations_staff",
                table: "conversations",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "IX_conversations_booking_id",
                table: "conversations",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_conversations_facility_id",
                table: "conversations",
                column: "facility_id");

            migrationBuilder.CreateIndex(
                name: "IX_dispute_attachments_dispute_id",
                table: "dispute_attachments",
                column: "dispute_id");

            migrationBuilder.CreateIndex(
                name: "IX_dispute_attachments_uploaded_by",
                table: "dispute_attachments",
                column: "uploaded_by");

            migrationBuilder.CreateIndex(
                name: "IX_disputes_admin_approval_id",
                table: "disputes",
                column: "admin_approval_id");

            migrationBuilder.CreateIndex(
                name: "IX_disputes_assigned_moderator_id",
                table: "disputes",
                column: "assigned_moderator_id");

            migrationBuilder.CreateIndex(
                name: "IX_disputes_booking_id",
                table: "disputes",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_disputes_raised_by",
                table: "disputes",
                column: "raised_by");

            migrationBuilder.CreateIndex(
                name: "idx_email_otps_email",
                table: "email_otps",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "idx_email_otps_expire",
                table: "email_otps",
                column: "expire_at");

            migrationBuilder.CreateIndex(
                name: "idx_facilities_location",
                table: "facilities",
                columns: new[] { "latitude", "longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_facilities_brand_id",
                table: "facilities",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_facility_amenities_amenity_id",
                table: "facility_amenities",
                column: "amenity_id");

            migrationBuilder.CreateIndex(
                name: "idx_facility_area_facility",
                table: "facility_area",
                column: "facility_id");

            migrationBuilder.CreateIndex(
                name: "ux_facility_area_name",
                table: "facility_area",
                columns: new[] { "facility_id", "area_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_host_addon_prices_facility",
                table: "host_addon_prices",
                column: "facility_id");

            migrationBuilder.CreateIndex(
                name: "IX_host_addon_prices_service_id",
                table: "host_addon_prices",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "ux_host_addon_service",
                table: "host_addon_prices",
                columns: new[] { "facility_id", "service_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_host_base_prices_facility",
                table: "host_base_prices",
                column: "facility_id");

            migrationBuilder.CreateIndex(
                name: "IX_host_base_prices_rule_id",
                table: "host_base_prices",
                column: "rule_id");

            migrationBuilder.CreateIndex(
                name: "IX_host_documents_reviewed_by",
                table: "host_documents",
                column: "reviewed_by");

            migrationBuilder.CreateIndex(
                name: "ux_host_documents_version",
                table: "host_documents",
                columns: new[] { "host_id", "document_type", "version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_host_primary_payout",
                table: "host_payout_accounts",
                column: "host_id",
                unique: true,
                filter: "(is_primary = true)");

            migrationBuilder.CreateIndex(
                name: "host_profiles_user_id_key",
                table: "host_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_host_profiles_verified_by",
                table: "host_profiles",
                column: "verified_by");

            migrationBuilder.CreateIndex(
                name: "idx_draft_email",
                table: "host_registration_drafts",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "idx_draft_expire",
                table: "host_registration_drafts",
                column: "expire_at");

            migrationBuilder.CreateIndex(
                name: "IX_host_registration_drafts_otp_id",
                table: "host_registration_drafts",
                column: "otp_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_media_assets_target",
                table: "media_assets",
                columns: new[] { "target_type", "target_id" });

            migrationBuilder.CreateIndex(
                name: "IX_media_assets_uploaded_by",
                table: "media_assets",
                column: "uploaded_by");

            migrationBuilder.CreateIndex(
                name: "ux_media_cover",
                table: "media_assets",
                columns: new[] { "target_type", "target_id" },
                unique: true,
                filter: "(is_cover = true)");

            migrationBuilder.CreateIndex(
                name: "idx_messages_conversation_time",
                table: "messages",
                columns: new[] { "conversation_id", "sent_at" });

            migrationBuilder.CreateIndex(
                name: "idx_messages_sender",
                table: "messages",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "idx_notifications_user_time",
                table: "notifications",
                columns: new[] { "user_id", "created_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "idx_payment_transactions_gateway",
                table: "payment_transactions",
                column: "gateway_transaction_id");

            migrationBuilder.CreateIndex(
                name: "idx_payment_transactions_payment",
                table: "payment_transactions",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "idx_payments_booking",
                table: "payments",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "idx_payments_status",
                table: "payments",
                column: "payment_status");

            migrationBuilder.CreateIndex(
                name: "payments_client_request_id_key",
                table: "payments",
                column: "client_request_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_platform_fee_code_effective",
                table: "platform_fee_configs",
                columns: new[] { "fee_code", "effective_from" });

            migrationBuilder.CreateIndex(
                name: "idx_platform_fee_target_host",
                table: "platform_fee_configs",
                column: "target_host_id");

            migrationBuilder.CreateIndex(
                name: "ux_platform_fee_code_time",
                table: "platform_fee_configs",
                columns: new[] { "fee_code", "effective_from" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_pricing_combo_rule_hours",
                table: "pricing_combos",
                columns: new[] { "rule_id", "hours" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_pricing_factors_rule",
                table: "pricing_factors",
                column: "rule_id");

            migrationBuilder.CreateIndex(
                name: "idx_reviews_facility",
                table: "reviews",
                column: "facility_id");

            migrationBuilder.CreateIndex(
                name: "idx_reviews_guest",
                table: "reviews",
                column: "guest_id");

            migrationBuilder.CreateIndex(
                name: "reviews_booking_id_key",
                table: "reviews",
                column: "booking_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_sleepbox_position",
                table: "sleepbox",
                columns: new[] { "area_id", "row_number", "level_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sleepbox_amenities_amenity_id",
                table: "sleepbox_amenities",
                column: "amenity_id");

            migrationBuilder.CreateIndex(
                name: "idx_staff_profiles_facility",
                table: "staff_profiles",
                column: "facility_id");

            migrationBuilder.CreateIndex(
                name: "idx_staff_profiles_user",
                table: "staff_profiles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ux_capacity_class",
                table: "system_box_type_price_limits",
                columns: new[] { "capacity_type", "box_class" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_system_policies_type",
                table: "system_policies",
                column: "policy_type");

            migrationBuilder.CreateIndex(
                name: "idx_price_rules_mode_active",
                table: "system_price_rules",
                columns: new[] { "pricing_mode", "is_active" });

            migrationBuilder.CreateIndex(
                name: "IX_user_favorites_facility_id",
                table: "user_favorites",
                column: "facility_id");

            migrationBuilder.CreateIndex(
                name: "ux_user_favorites",
                table: "user_favorites",
                columns: new[] { "user_id", "facility_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "user_profiles_user_id_key",
                table: "user_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_email_key",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_username_key",
                table: "users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_wallet_transactions_reference",
                table: "wallet_transactions",
                columns: new[] { "reference_type", "reference_id" });

            migrationBuilder.CreateIndex(
                name: "idx_wallet_transactions_type",
                table: "wallet_transactions",
                column: "transaction_type");

            migrationBuilder.CreateIndex(
                name: "idx_wallet_transactions_wallet_time",
                table: "wallet_transactions",
                columns: new[] { "wallet_id", "created_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "idx_wallets_host",
                table: "wallets",
                column: "host_id");

            migrationBuilder.CreateIndex(
                name: "idx_wallets_type",
                table: "wallets",
                column: "wallet_type");

            migrationBuilder.CreateIndex(
                name: "idx_withdrawal_requests_host",
                table: "withdrawal_requests",
                column: "host_id");

            migrationBuilder.CreateIndex(
                name: "idx_withdrawal_requests_status",
                table: "withdrawal_requests",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "idx_withdrawal_requests_wallet",
                table: "withdrawal_requests",
                column: "wallet_id");

            migrationBuilder.CreateIndex(
                name: "IX_withdrawal_requests_account_id",
                table: "withdrawal_requests",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "withdrawal_requests_request_code_key",
                table: "withdrawal_requests",
                column: "request_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "booking_addon_services");

            migrationBuilder.DropTable(
                name: "booking_boxes");

            migrationBuilder.DropTable(
                name: "booking_status_history");

            migrationBuilder.DropTable(
                name: "box_availability");

            migrationBuilder.DropTable(
                name: "dispute_attachments");

            migrationBuilder.DropTable(
                name: "facility_amenities");

            migrationBuilder.DropTable(
                name: "host_addon_prices");

            migrationBuilder.DropTable(
                name: "host_base_prices");

            migrationBuilder.DropTable(
                name: "host_documents");

            migrationBuilder.DropTable(
                name: "host_registration_drafts");

            migrationBuilder.DropTable(
                name: "media_assets");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "payment_transactions");

            migrationBuilder.DropTable(
                name: "platform_fee_configs");

            migrationBuilder.DropTable(
                name: "pricing_combos");

            migrationBuilder.DropTable(
                name: "pricing_factors");

            migrationBuilder.DropTable(
                name: "reviews");

            migrationBuilder.DropTable(
                name: "sleepbox_amenities");

            migrationBuilder.DropTable(
                name: "system_box_type_price_limits");

            migrationBuilder.DropTable(
                name: "system_policies");

            migrationBuilder.DropTable(
                name: "user_favorites");

            migrationBuilder.DropTable(
                name: "user_profiles");

            migrationBuilder.DropTable(
                name: "wallet_transactions");

            migrationBuilder.DropTable(
                name: "withdrawal_requests");

            migrationBuilder.DropTable(
                name: "disputes");

            migrationBuilder.DropTable(
                name: "addon_services");

            migrationBuilder.DropTable(
                name: "email_otps");

            migrationBuilder.DropTable(
                name: "conversations");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "system_price_rules");

            migrationBuilder.DropTable(
                name: "amenities");

            migrationBuilder.DropTable(
                name: "sleepbox");

            migrationBuilder.DropTable(
                name: "host_payout_accounts");

            migrationBuilder.DropTable(
                name: "wallets");

            migrationBuilder.DropTable(
                name: "staff_profiles");

            migrationBuilder.DropTable(
                name: "bookings");

            migrationBuilder.DropTable(
                name: "facility_area");

            migrationBuilder.DropTable(
                name: "facilities");

            migrationBuilder.DropTable(
                name: "brands");

            migrationBuilder.DropTable(
                name: "host_profiles");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
