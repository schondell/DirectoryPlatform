using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    icon_url = table.Column<string>(type: "text", nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    meta_title = table.Column<string>(type: "text", nullable: true),
                    meta_description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_categories_categories_parent_id",
                        column: x => x.parent_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "coupon_codes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    discount_percent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    max_discount_amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    valid_from = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    valid_to = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    max_uses = table.Column<int>(type: "integer", nullable: true),
                    used_count = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_coupon_codes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "languages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    native_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_languages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pricing_tiers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    duration_days = table.Column<int>(type: "integer", nullable: false),
                    max_listings = table.Column<int>(type: "integer", nullable: false),
                    max_photos = table.Column<int>(type: "integer", nullable: false),
                    is_featured = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pricing_tiers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "regions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    country_code = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    meta_title = table.Column<string>(type: "text", nullable: true),
                    meta_description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_regions", x => x.id);
                    table.ForeignKey(
                        name: "fk_regions_regions_parent_id",
                        column: x => x.parent_id,
                        principalTable: "regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "subscription_tiers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    monthly_price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    annual_price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscription_tiers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    password_salt = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    is_email_verified = table.Column<bool>(type: "boolean", nullable: false),
                    email_verification_token = table.Column<string>(type: "text", nullable: true),
                    email_verification_token_expiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    password_reset_token = table.Column<string>(type: "text", nullable: true),
                    password_reset_token_expiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_locked = table.Column<bool>(type: "boolean", nullable: false),
                    failed_login_attempts = table.Column<int>(type: "integer", nullable: false),
                    lockout_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    refresh_token = table.Column<string>(type: "text", nullable: true),
                    refresh_token_expiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    profile_image_url = table.Column<string>(type: "text", nullable: true),
                    first_name = table.Column<string>(type: "text", nullable: true),
                    last_name = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    owner_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_users_owner_user_id",
                        column: x => x.owner_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "visitor_metrics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    unique_visitors = table.Column<int>(type: "integer", nullable: false),
                    total_page_views = table.Column<int>(type: "integer", nullable: false),
                    new_users = table.Column<int>(type: "integer", nullable: false),
                    new_listings = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_visitor_metrics", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "attribute_definitions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    options = table.Column<string>(type: "jsonb", nullable: true),
                    is_filterable = table.Column<bool>(type: "boolean", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    min_value = table.Column<double>(type: "double precision", nullable: true),
                    max_value = table.Column<double>(type: "double precision", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attribute_definitions", x => x.id);
                    table.ForeignKey(
                        name: "fk_attribute_definitions_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subscription_features",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subscription_tier_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    value = table.Column<string>(type: "text", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscription_features", x => x.id);
                    table.ForeignKey(
                        name: "fk_subscription_features_subscription_tiers_subscription_tier_",
                        column: x => x.subscription_tier_id,
                        principalTable: "subscription_tiers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entity_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    old_values = table.Column<string>(type: "jsonb", nullable: true),
                    new_values = table.Column<string>(type: "jsonb", nullable: true),
                    ip_address = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_audit_logs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "listings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    short_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    region_id = table.Column<Guid>(type: "uuid", nullable: true),
                    town = table.Column<string>(type: "text", nullable: true),
                    weight = table.Column<int>(type: "integer", nullable: false),
                    is_featured = table.Column<bool>(type: "boolean", nullable: false),
                    is_premium = table.Column<bool>(type: "boolean", nullable: false),
                    view_count = table.Column<int>(type: "integer", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_listings", x => x.id);
                    table.ForeignKey(
                        name: "fk_listings_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_listings_regions_region_id",
                        column: x => x.region_id,
                        principalTable: "regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_listings_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    action_url = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notifications", x => x.id);
                    table.ForeignKey(
                        name: "fk_notifications_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subscription_tier_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    auto_renew = table.Column<bool>(type: "boolean", nullable: false),
                    payment_reference = table.Column<string>(type: "text", nullable: true),
                    coupon_code_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscriptions", x => x.id);
                    table.ForeignKey(
                        name: "fk_subscriptions_coupon_codes_coupon_code_id",
                        column: x => x.coupon_code_id,
                        principalTable: "coupon_codes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_subscriptions_subscription_tiers_subscription_tier_id",
                        column: x => x.subscription_tier_id,
                        principalTable: "subscription_tiers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_subscriptions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "listing_approval_histories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reviewed_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    old_status = table.Column<int>(type: "integer", nullable: false),
                    new_status = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_listing_approval_histories", x => x.id);
                    table.ForeignKey(
                        name: "fk_listing_approval_histories_listings_listing_id",
                        column: x => x.listing_id,
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_listing_approval_histories_users_reviewed_by_user_id",
                        column: x => x.reviewed_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "listing_attributes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attribute_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_listing_attributes", x => x.id);
                    table.ForeignKey(
                        name: "fk_listing_attributes_attribute_definitions_attribute_definiti",
                        column: x => x.attribute_definition_id,
                        principalTable: "attribute_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_listing_attributes_listings_listing_id",
                        column: x => x.listing_id,
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "listing_boosts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    boost_type = table.Column<int>(type: "integer", nullable: false),
                    starts_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    multiplier = table.Column<double>(type: "double precision", nullable: false),
                    amount_paid = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_listing_boosts", x => x.id);
                    table.ForeignKey(
                        name: "fk_listing_boosts_listings_listing_id",
                        column: x => x.listing_id,
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_listing_boosts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "listing_details",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    latitude = table.Column<double>(type: "double precision", nullable: true),
                    longitude = table.Column<double>(type: "double precision", nullable: true),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    website = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    availability_hours = table.Column<string>(type: "jsonb", nullable: true),
                    price_info = table.Column<string>(type: "jsonb", nullable: true),
                    payment_methods = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_listing_details", x => x.id);
                    table.ForeignKey(
                        name: "fk_listing_details_listings_listing_id",
                        column: x => x.listing_id,
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "listing_followers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notify_on_update = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_listing_followers", x => x.id);
                    table.ForeignKey(
                        name: "fk_listing_followers_listings_listing_id",
                        column: x => x.listing_id,
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_listing_followers_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "listing_languages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_listing_languages", x => x.id);
                    table.ForeignKey(
                        name: "fk_listing_languages_languages_language_id",
                        column: x => x.language_id,
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_listing_languages_listings_listing_id",
                        column: x => x.listing_id,
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "listing_likes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_listing_likes", x => x.id);
                    table.ForeignKey(
                        name: "fk_listing_likes_listings_listing_id",
                        column: x => x.listing_id,
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_listing_likes_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "listing_media",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    media_type = table.Column<int>(type: "integer", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    alt_text = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_listing_media", x => x.id);
                    table.ForeignKey(
                        name: "fk_listing_media_listings_listing_id",
                        column: x => x.listing_id,
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "listing_page_views",
                columns: table => new
                {
                    listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    view_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    view_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_listing_page_views", x => new { x.listing_id, x.view_date });
                    table.ForeignKey(
                        name: "fk_listing_page_views_listings_listing_id",
                        column: x => x.listing_id,
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "listing_visitors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    visited_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_listing_visitors", x => x.id);
                    table.ForeignKey(
                        name: "fk_listing_visitors_listings_listing_id",
                        column: x => x.listing_id,
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_listing_visitors_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: true),
                    subject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted_by_sender = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted_by_recipient = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_messages_listings_listing_id",
                        column: x => x.listing_id,
                        principalTable: "listings",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_messages_users_recipient_id",
                        column: x => x.recipient_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_messages_users_sender_id",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reviews",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reviews", x => x.id);
                    table.ForeignKey(
                        name: "fk_reviews_listings_listing_id",
                        column: x => x.listing_id,
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reviews_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "invoices",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: true),
                    subscription_id = table.Column<Guid>(type: "uuid", nullable: true),
                    subtotal = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    issue_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    paid_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoices", x => x.id);
                    table.ForeignKey(
                        name: "fk_invoices_listings_listing_id",
                        column: x => x.listing_id,
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_invoices_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_invoices_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "invoice_line_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    total_price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoice_line_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_invoice_line_items_invoices_invoice_id",
                        column: x => x.invoice_id,
                        principalTable: "invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    payment_method = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    transaction_reference = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payments", x => x.id);
                    table.ForeignKey(
                        name: "fk_payments_invoices_invoice_id",
                        column: x => x.invoice_id,
                        principalTable: "invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payment_reconciliations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recorded_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_reconciliations", x => x.id);
                    table.ForeignKey(
                        name: "fk_payment_reconciliations_payments_payment_id",
                        column: x => x.payment_id,
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_payment_reconciliations_users_recorded_by_user_id",
                        column: x => x.recorded_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_attribute_definitions_category_id_slug",
                table: "attribute_definitions",
                columns: new[] { "category_id", "slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_entity_type_entity_id",
                table: "audit_logs",
                columns: new[] { "entity_type", "entity_id" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_user_id",
                table: "audit_logs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_categories_parent_id",
                table: "categories",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_categories_slug",
                table: "categories",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_coupon_codes_code",
                table: "coupon_codes",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_invoice_line_items_invoice_id",
                table: "invoice_line_items",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoices_invoice_number",
                table: "invoices",
                column: "invoice_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_invoices_listing_id",
                table: "invoices",
                column: "listing_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoices_subscription_id",
                table: "invoices",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoices_user_id",
                table: "invoices",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_languages_code",
                table: "languages",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_listing_approval_histories_listing_id",
                table: "listing_approval_histories",
                column: "listing_id");

            migrationBuilder.CreateIndex(
                name: "ix_listing_approval_histories_reviewed_by_user_id",
                table: "listing_approval_histories",
                column: "reviewed_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_listing_attributes_attribute_definition_id_value",
                table: "listing_attributes",
                columns: new[] { "attribute_definition_id", "value" });

            migrationBuilder.CreateIndex(
                name: "ix_listing_attributes_listing_id",
                table: "listing_attributes",
                column: "listing_id");

            migrationBuilder.CreateIndex(
                name: "ix_listing_boosts_listing_id_starts_at_expires_at",
                table: "listing_boosts",
                columns: new[] { "listing_id", "starts_at", "expires_at" });

            migrationBuilder.CreateIndex(
                name: "ix_listing_boosts_user_id",
                table: "listing_boosts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_listing_details_listing_id",
                table: "listing_details",
                column: "listing_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_listing_followers_listing_id_user_id",
                table: "listing_followers",
                columns: new[] { "listing_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_listing_followers_user_id",
                table: "listing_followers",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_listing_languages_language_id",
                table: "listing_languages",
                column: "language_id");

            migrationBuilder.CreateIndex(
                name: "ix_listing_languages_listing_id_language_id",
                table: "listing_languages",
                columns: new[] { "listing_id", "language_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_listing_likes_listing_id_user_id",
                table: "listing_likes",
                columns: new[] { "listing_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_listing_likes_user_id",
                table: "listing_likes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_listing_media_listing_id",
                table: "listing_media",
                column: "listing_id");

            migrationBuilder.CreateIndex(
                name: "ix_listing_visitors_listing_id_user_id",
                table: "listing_visitors",
                columns: new[] { "listing_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_listing_visitors_user_id",
                table: "listing_visitors",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_listings_category_id",
                table: "listings",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_listings_region_id",
                table: "listings",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_listings_status",
                table: "listings",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_listings_status_weight",
                table: "listings",
                columns: new[] { "status", "weight" });

            migrationBuilder.CreateIndex(
                name: "ix_listings_user_id",
                table: "listings",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_listing_id",
                table: "messages",
                column: "listing_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_recipient_id",
                table: "messages",
                column: "recipient_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_sender_id",
                table: "messages",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_user_id_is_read",
                table: "notifications",
                columns: new[] { "user_id", "is_read" });

            migrationBuilder.CreateIndex(
                name: "ix_payment_reconciliations_payment_id",
                table: "payment_reconciliations",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_reconciliations_recorded_by_user_id",
                table: "payment_reconciliations",
                column: "recorded_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_payments_invoice_id",
                table: "payments",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_regions_parent_id",
                table: "regions",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_regions_slug",
                table: "regions",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reviews_listing_id",
                table: "reviews",
                column: "listing_id");

            migrationBuilder.CreateIndex(
                name: "ix_reviews_user_id",
                table: "reviews",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscription_features_subscription_tier_id",
                table: "subscription_features",
                column: "subscription_tier_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_coupon_code_id",
                table: "subscriptions",
                column: "coupon_code_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_subscription_tier_id",
                table: "subscriptions",
                column: "subscription_tier_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_user_id_is_active",
                table: "subscriptions",
                columns: new[] { "user_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_owner_user_id",
                table: "users",
                column: "owner_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_refresh_token",
                table: "users",
                column: "refresh_token",
                unique: true,
                filter: "refresh_token IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_users_username",
                table: "users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_visitor_metrics_date",
                table: "visitor_metrics",
                column: "date",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "invoice_line_items");

            migrationBuilder.DropTable(
                name: "listing_approval_histories");

            migrationBuilder.DropTable(
                name: "listing_attributes");

            migrationBuilder.DropTable(
                name: "listing_boosts");

            migrationBuilder.DropTable(
                name: "listing_details");

            migrationBuilder.DropTable(
                name: "listing_followers");

            migrationBuilder.DropTable(
                name: "listing_languages");

            migrationBuilder.DropTable(
                name: "listing_likes");

            migrationBuilder.DropTable(
                name: "listing_media");

            migrationBuilder.DropTable(
                name: "listing_page_views");

            migrationBuilder.DropTable(
                name: "listing_visitors");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "payment_reconciliations");

            migrationBuilder.DropTable(
                name: "pricing_tiers");

            migrationBuilder.DropTable(
                name: "reviews");

            migrationBuilder.DropTable(
                name: "subscription_features");

            migrationBuilder.DropTable(
                name: "visitor_metrics");

            migrationBuilder.DropTable(
                name: "attribute_definitions");

            migrationBuilder.DropTable(
                name: "languages");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "invoices");

            migrationBuilder.DropTable(
                name: "listings");

            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "regions");

            migrationBuilder.DropTable(
                name: "coupon_codes");

            migrationBuilder.DropTable(
                name: "subscription_tiers");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
