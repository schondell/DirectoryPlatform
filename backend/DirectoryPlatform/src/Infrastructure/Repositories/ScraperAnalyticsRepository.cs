using System.Data;
using DirectoryPlatform.Contracts.DTOs.ScraperAnalytics;
using DirectoryPlatform.Contracts.Services;
using DirectoryPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DirectoryPlatform.Infrastructure.Repositories;

public class ScraperAnalyticsRepository : IScraperAnalyticsRepository
{
    private readonly string _connectionString;

    public ScraperAnalyticsRepository(ScraperDbContext context)
    {
        _connectionString = context.Database.GetConnectionString()
            ?? throw new InvalidOperationException("ScraperDbContext has no connection string");
    }

    public async Task<ScraperOverviewDto> GetOverviewAsync()
    {
        const string sql = """
            SELECT
                count(*) as total,
                count(*) FILTER (WHERE status = 'active') as active,
                count(*) FILTER (WHERE status = 'expired') as expired,
                count(*) FILTER (WHERE is_paid = true) as paid,
                count(*) FILTER (WHERE phone_normalized IS NOT NULL) as with_phone,
                count(*) FILTER (WHERE images_downloaded = true) as with_images,
                count(DISTINCT phone_normalized) FILTER (WHERE phone_normalized IS NOT NULL) as unique_phones,
                count(DISTINCT category_external_id) as categories_with_listings,
                min(first_seen_at) as oldest_listing,
                max(first_seen_at) as newest_listing
            FROM petitesannonces_listings
            """;

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new ScraperOverviewDto
            {
                Total = reader.GetInt32("total"),
                Active = reader.GetInt32("active"),
                Expired = reader.GetInt32("expired"),
                Paid = reader.GetInt32("paid"),
                WithPhone = reader.GetInt32("with_phone"),
                WithImages = reader.GetInt32("with_images"),
                UniquePhones = reader.GetInt32("unique_phones"),
                Categories = reader.GetInt32("categories_with_listings"),
                OldestListing = reader.IsDBNull("oldest_listing") ? null : reader.GetDateTime("oldest_listing"),
                NewestListing = reader.IsDBNull("newest_listing") ? null : reader.GetDateTime("newest_listing")
            };
        }

        return new ScraperOverviewDto();
    }

    public async Task<List<LifecycleEntryDto>> GetLifecycleAsync()
    {
        const string sql = """
            SELECT
                c.name as category_name,
                count(*) as expired_count,
                round(avg(extract(epoch from (expired_at - first_seen_at)) / 86400)::numeric, 1) as avg_days_alive,
                round(min(extract(epoch from (expired_at - first_seen_at)) / 86400)::numeric, 1) as min_days,
                round(max(extract(epoch from (expired_at - first_seen_at)) / 86400)::numeric, 1) as max_days,
                round(percentile_cont(0.5) WITHIN GROUP (
                    ORDER BY extract(epoch from (expired_at - first_seen_at)) / 86400
                )::numeric, 1) as median_days
            FROM petitesannonces_listings l
            LEFT JOIN petitesannonces_categories c ON c.external_id = l.category_external_id
            WHERE status = 'expired' AND expired_at IS NOT NULL AND first_seen_at IS NOT NULL
            GROUP BY l.category_external_id, c.name
            HAVING count(*) >= 5
            ORDER BY avg_days_alive DESC
            """;

        return await ExecuteListAsync(sql, reader => new LifecycleEntryDto
        {
            Category = reader.IsDBNull("category_name") ? "Unknown" : reader.GetString("category_name"),
            ExpiredCount = reader.GetInt32("expired_count"),
            AvgDaysAlive = reader.GetDouble("avg_days_alive"),
            MinDays = reader.GetDouble("min_days"),
            MaxDays = reader.GetDouble("max_days"),
            MedianDays = reader.GetDouble("median_days")
        });
    }

    public async Task<List<VelocityEntryDto>> GetVelocityAsync()
    {
        const string sql = """
            SELECT
                c.name as category_name,
                count(*) as total_listings,
                count(*) FILTER (WHERE l.first_seen_at > NOW() - interval '7 days') as last_7d,
                count(*) FILTER (WHERE l.first_seen_at > NOW() - interval '1 day') as last_24h,
                round(count(*)::numeric / GREATEST(
                    extract(epoch from (max(l.first_seen_at) - min(l.first_seen_at))) / 86400, 1
                ), 1) as avg_per_day
            FROM petitesannonces_listings l
            LEFT JOIN petitesannonces_categories c ON c.external_id = l.category_external_id
            GROUP BY l.category_external_id, c.name
            ORDER BY avg_per_day DESC
            LIMIT 30
            """;

        return await ExecuteListAsync(sql, reader => new VelocityEntryDto
        {
            Category = reader.IsDBNull("category_name") ? "Unknown" : reader.GetString("category_name"),
            TotalListings = reader.GetInt32("total_listings"),
            Last7d = reader.GetInt32("last_7d"),
            Last24h = reader.GetInt32("last_24h"),
            AvgPerDay = reader.GetDouble("avg_per_day")
        });
    }

    public async Task<List<DealerEntryDto>> GetDealersAsync()
    {
        const string sql = """
            SELECT
                phone_normalized,
                count(*) as listing_count,
                count(DISTINCT category_external_id) as category_count,
                count(*) FILTER (WHERE is_paid = true) as paid_count,
                count(*) FILTER (WHERE status = 'active') as active_count,
                array_agg(DISTINCT (parsed_data->>'seller_pseudo'))
                    FILTER (WHERE parsed_data->>'seller_pseudo' IS NOT NULL
                            AND parsed_data->>'seller_pseudo' != '') as pseudos,
                array_agg(DISTINCT (parsed_data->>'location'))
                    FILTER (WHERE parsed_data->>'location' IS NOT NULL
                            AND parsed_data->>'location' != '') as locations
            FROM petitesannonces_listings
            WHERE phone_normalized IS NOT NULL
            GROUP BY phone_normalized
            HAVING count(*) >= 3
            ORDER BY listing_count DESC
            LIMIT 30
            """;

        return await ExecuteListAsync(sql, reader => new DealerEntryDto
        {
            Phone = reader.GetString("phone_normalized"),
            ListingCount = reader.GetInt32("listing_count"),
            CategoryCount = reader.GetInt32("category_count"),
            PaidCount = reader.GetInt32("paid_count"),
            ActiveCount = reader.GetInt32("active_count"),
            Pseudos = ReadStringArray(reader, "pseudos"),
            Locations = ReadStringArray(reader, "locations")
        });
    }

    public async Task<List<RepostEntryDto>> GetRepostsAsync()
    {
        const string sql = """
            WITH seller_timeline AS (
                SELECT
                    phone_normalized,
                    category_external_id,
                    first_seen_at,
                    expired_at,
                    parsed_data->>'title' as title,
                    LAG(expired_at) OVER (
                        PARTITION BY phone_normalized, category_external_id
                        ORDER BY first_seen_at
                    ) as prev_expired_at
                FROM petitesannonces_listings
                WHERE phone_normalized IS NOT NULL
            )
            SELECT
                phone_normalized,
                category_external_id,
                count(*) as repost_count,
                round(avg(
                    CASE WHEN prev_expired_at IS NOT NULL THEN
                        extract(epoch from (first_seen_at - prev_expired_at)) / 3600
                    END
                )::numeric, 1) as avg_hours_between_reposts,
                min(title) as sample_title
            FROM seller_timeline
            WHERE prev_expired_at IS NOT NULL
            GROUP BY phone_normalized, category_external_id
            HAVING count(*) >= 2
            ORDER BY repost_count DESC
            LIMIT 20
            """;

        return await ExecuteListAsync(sql, reader => new RepostEntryDto
        {
            Phone = reader.GetString("phone_normalized"),
            Category = reader.IsDBNull("category_external_id") ? null : reader.GetString("category_external_id"),
            RepostCount = reader.GetInt32("repost_count"),
            AvgHoursBetween = reader.IsDBNull("avg_hours_between_reposts") ? null : reader.GetDouble("avg_hours_between_reposts"),
            SampleTitle = reader.IsDBNull("sample_title") ? null : reader.GetString("sample_title")
        });
    }

    public async Task<List<PaidVsFreeEntryDto>> GetPaidVsFreeAsync()
    {
        const string sql = """
            SELECT
                is_paid,
                paid_type,
                count(*) as count,
                count(*) FILTER (WHERE status = 'active') as active,
                count(*) FILTER (WHERE status = 'expired') as expired,
                round(avg(
                    CASE WHEN status = 'expired' AND expired_at IS NOT NULL THEN
                        extract(epoch from (expired_at - first_seen_at)) / 86400
                    END
                )::numeric, 1) as avg_days_alive,
                count(*) FILTER (WHERE phone_normalized IS NOT NULL) as with_phone,
                round(avg(
                    COALESCE((parsed_data->>'image_count')::int, 0)
                )::numeric, 1) as avg_images
            FROM petitesannonces_listings
            GROUP BY is_paid, paid_type
            ORDER BY count DESC
            """;

        return await ExecuteListAsync(sql, reader => new PaidVsFreeEntryDto
        {
            IsPaid = reader.GetBoolean("is_paid"),
            PaidType = reader.IsDBNull("paid_type") ? null : reader.GetString("paid_type"),
            Count = reader.GetInt32("count"),
            Active = reader.GetInt32("active"),
            Expired = reader.GetInt32("expired"),
            AvgDaysAlive = reader.IsDBNull("avg_days_alive") ? null : reader.GetDouble("avg_days_alive"),
            WithPhone = reader.GetInt32("with_phone"),
            AvgImages = reader.IsDBNull("avg_images") ? 0 : reader.GetDouble("avg_images")
        });
    }

    public async Task<List<PriceDistributionEntryDto>> GetPriceDistributionAsync()
    {
        const string sql = """
            WITH prices AS (
                SELECT
                    l.category_external_id,
                    c.name as category_name,
                    regexp_replace(
                        regexp_replace(parsed_data->>'price', '[^0-9.]', '', 'g'),
                        '^\\.+|\\.+$', '', 'g'
                    ) as price_str
                FROM petitesannonces_listings l
                LEFT JOIN petitesannonces_categories c ON c.external_id = l.category_external_id
                WHERE parsed_data->>'price' IS NOT NULL
                  AND parsed_data->>'price' != ''
            ),
            parsed AS (
                SELECT category_external_id, category_name,
                       CASE WHEN price_str ~ '^\d+\.?\d*$'
                            THEN price_str::numeric ELSE NULL END as price
                FROM prices
            )
            SELECT
                category_name,
                count(*) FILTER (WHERE price IS NOT NULL) as count,
                round(avg(price)::numeric, 0) as avg_price,
                round(percentile_cont(0.5) WITHIN GROUP (ORDER BY price)::numeric, 0) as median_price,
                round(min(price)::numeric, 0) as min_price,
                round(max(price)::numeric, 0) as max_price
            FROM parsed
            WHERE price IS NOT NULL AND price > 0 AND price < 10000000
            GROUP BY category_external_id, category_name
            HAVING count(*) FILTER (WHERE price IS NOT NULL) >= 5
            ORDER BY count(*) DESC
            LIMIT 20
            """;

        return await ExecuteListAsync(sql, reader => new PriceDistributionEntryDto
        {
            Category = reader.IsDBNull("category_name") ? "Unknown" : reader.GetString("category_name"),
            Count = reader.GetInt32("count"),
            Avg = reader.GetDecimal("avg_price"),
            Median = reader.GetDecimal("median_price"),
            Min = reader.GetDecimal("min_price"),
            Max = reader.GetDecimal("max_price")
        });
    }

    public async Task<List<GeoEntryDto>> GetGeographicAsync()
    {
        const string sql = """
            SELECT
                parsed_data->>'location' as location,
                count(*) as listing_count,
                count(*) FILTER (WHERE status = 'active') as active,
                count(DISTINCT phone_normalized) FILTER (WHERE phone_normalized IS NOT NULL) as unique_sellers
            FROM petitesannonces_listings
            WHERE parsed_data->>'location' IS NOT NULL
              AND parsed_data->>'location' != ''
            GROUP BY parsed_data->>'location'
            ORDER BY listing_count DESC
            LIMIT 25
            """;

        return await ExecuteListAsync(sql, reader => new GeoEntryDto
        {
            Location = reader.GetString("location"),
            ListingCount = reader.GetInt32("listing_count"),
            Active = reader.GetInt32("active"),
            UniqueSellers = reader.GetInt32("unique_sellers")
        });
    }

    public async Task<ScraperFreshnessDto> GetFreshnessAsync()
    {
        const string sql = """
            SELECT
                count(*) as total_active,
                count(*) FILTER (WHERE last_seen_at > NOW() - interval '1 day') as seen_24h,
                count(*) FILTER (WHERE last_seen_at > NOW() - interval '7 days') as seen_7d,
                count(*) FILTER (WHERE last_seen_at > NOW() - interval '30 days') as seen_30d,
                count(*) FILTER (WHERE last_seen_at <= NOW() - interval '30 days') as stale_30d,
                round(avg(extract(epoch from (NOW() - last_seen_at)) / 86400)::numeric, 1) as avg_days_since_seen
            FROM petitesannonces_listings
            WHERE status = 'active'
            """;

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new ScraperFreshnessDto
            {
                TotalActive = reader.GetInt32("total_active"),
                Seen24h = reader.GetInt32("seen_24h"),
                Seen7d = reader.GetInt32("seen_7d"),
                Seen30d = reader.GetInt32("seen_30d"),
                Stale30d = reader.GetInt32("stale_30d"),
                AvgDaysSinceSeen = reader.IsDBNull("avg_days_since_seen") ? 0 : reader.GetDouble("avg_days_since_seen")
            };
        }

        return new ScraperFreshnessDto();
    }

    private async Task<List<T>> ExecuteListAsync<T>(string sql, Func<NpgsqlDataReader, T> map)
    {
        var results = new List<T>();
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            results.Add(map(reader));
        }

        return results;
    }

    private static List<string> ReadStringArray(NpgsqlDataReader reader, string column)
    {
        if (reader.IsDBNull(column))
            return [];

        var value = reader.GetValue(reader.GetOrdinal(column));
        if (value is string[] arr)
            return arr.Where(s => !string.IsNullOrEmpty(s)).ToList();

        return [];
    }

    private static double GetDoubleOrDefault(NpgsqlDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);
        if (reader.IsDBNull(ordinal)) return 0;
        return Convert.ToDouble(reader.GetValue(ordinal));
    }
}
