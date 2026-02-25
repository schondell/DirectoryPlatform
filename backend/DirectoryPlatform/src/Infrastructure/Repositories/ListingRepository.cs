using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Enums;
using DirectoryPlatform.Core.Interfaces;
using DirectoryPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.Infrastructure.Repositories;

public class ListingRepository : Repository<Listing>, IListingRepository
{
    public ListingRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Listing?> GetWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(l => l.Category)
            .Include(l => l.Region)
            .Include(l => l.Detail)
            .Include(l => l.Media.OrderBy(m => m.DisplayOrder))
            .Include(l => l.Attributes).ThenInclude(a => a.AttributeDefinition)
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<(IEnumerable<Listing> Items, int TotalCount)> GetFilteredAsync(ListingFilterParams filter)
    {
        var query = _dbSet
            .Include(l => l.Category)
            .Include(l => l.Region)
            .Include(l => l.Media.OrderBy(m => m.DisplayOrder))
            .Include(l => l.Attributes).ThenInclude(a => a.AttributeDefinition)
            .Where(l => l.Status == ListingStatus.Active)
            .AsQueryable();

        // Text search
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.ToLower();
            query = query.Where(l =>
                l.Title.ToLower().Contains(term) ||
                (l.ShortDescription != null && l.ShortDescription.ToLower().Contains(term)) ||
                (l.Description != null && l.Description.ToLower().Contains(term)));
        }

        // Category filter
        if (filter.CategoryId.HasValue)
        {
            var categoryIds = await GetCategoryAndChildIds(filter.CategoryId.Value);
            query = query.Where(l => categoryIds.Contains(l.CategoryId));
        }

        // Region filter
        if (filter.RegionId.HasValue)
        {
            var regionIds = await GetRegionAndChildIds(filter.RegionId.Value);
            query = query.Where(l => regionIds.Contains(l.RegionId!.Value));
        }

        // Dynamic attribute filters
        foreach (var (slug, value) in filter.AttributeFilters)
        {
            if (string.IsNullOrWhiteSpace(value)) continue;

            // Get attribute definition to determine type
            var attrDef = await _context.AttributeDefinitions
                .FirstOrDefaultAsync(a => a.Slug == slug);

            if (attrDef == null) continue;

            switch (attrDef.Type)
            {
                case AttributeType.Boolean:
                    query = query.Where(l => l.Attributes.Any(a =>
                        a.AttributeDefinition.Slug == slug && a.Value == value));
                    break;

                case AttributeType.Number:
                    query = ApplyNumberFilter(query, slug, value);
                    break;

                case AttributeType.MultiSelect:
                    var selectedValues = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    query = query.Where(l => l.Attributes.Any(a =>
                        a.AttributeDefinition.Slug == slug &&
                        selectedValues.Any(sv => a.Value.Contains(sv))));
                    break;

                default: // Text, Select, Date
                    query = query.Where(l => l.Attributes.Any(a =>
                        a.AttributeDefinition.Slug == slug && a.Value == value));
                    break;
            }
        }

        var totalCount = await query.CountAsync();

        // Sorting
        query = filter.SortBy?.ToLower() switch
        {
            "title" => filter.Ascending ? query.OrderBy(l => l.Title) : query.OrderByDescending(l => l.Title),
            "createdat" => filter.Ascending ? query.OrderBy(l => l.CreatedAt) : query.OrderByDescending(l => l.CreatedAt),
            "viewcount" => filter.Ascending ? query.OrderBy(l => l.ViewCount) : query.OrderByDescending(l => l.ViewCount),
            "weight" => filter.Ascending ? query.OrderBy(l => l.Weight) : query.OrderByDescending(l => l.Weight),
            _ => query.OrderByDescending(l => l.Weight).ThenByDescending(l => l.CreatedAt)
        };

        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    private static IQueryable<Listing> ApplyNumberFilter(IQueryable<Listing> query, string slug, string value)
    {
        // Support formats: "100-500", "min:100", "max:500"
        if (value.Contains('-'))
        {
            var parts = value.Split('-');
            if (double.TryParse(parts[0], out var min) && double.TryParse(parts[1], out var max))
            {
                query = query.Where(l => l.Attributes.Any(a =>
                    a.AttributeDefinition.Slug == slug &&
                    Convert.ToDouble(a.Value) >= min &&
                    Convert.ToDouble(a.Value) <= max));
            }
        }
        else if (value.StartsWith("min:") && double.TryParse(value[4..], out var minVal))
        {
            query = query.Where(l => l.Attributes.Any(a =>
                a.AttributeDefinition.Slug == slug &&
                Convert.ToDouble(a.Value) >= minVal));
        }
        else if (value.StartsWith("max:") && double.TryParse(value[4..], out var maxVal))
        {
            query = query.Where(l => l.Attributes.Any(a =>
                a.AttributeDefinition.Slug == slug &&
                Convert.ToDouble(a.Value) <= maxVal));
        }
        else if (double.TryParse(value, out _))
        {
            query = query.Where(l => l.Attributes.Any(a =>
                a.AttributeDefinition.Slug == slug && a.Value == value));
        }

        return query;
    }

    private async Task<List<Guid>> GetCategoryAndChildIds(Guid categoryId)
    {
        var ids = new List<Guid> { categoryId };
        var children = await _context.Categories.Where(c => c.ParentId == categoryId).Select(c => c.Id).ToListAsync();
        ids.AddRange(children);
        foreach (var childId in children)
        {
            var grandChildren = await _context.Categories.Where(c => c.ParentId == childId).Select(c => c.Id).ToListAsync();
            ids.AddRange(grandChildren);
        }
        return ids;
    }

    private async Task<List<Guid>> GetRegionAndChildIds(Guid regionId)
    {
        var ids = new List<Guid> { regionId };
        var children = await _context.Regions.Where(r => r.ParentId == regionId).Select(r => r.Id).ToListAsync();
        ids.AddRange(children);
        return ids;
    }

    public async Task<IEnumerable<Listing>> GetByUserIdAsync(Guid userId)
        => await _dbSet.Include(l => l.Category).Include(l => l.Media).Where(l => l.UserId == userId).OrderByDescending(l => l.CreatedAt).ToListAsync();

    public async Task<IEnumerable<Listing>> GetFeaturedAsync(int count)
        => await _dbSet.Include(l => l.Category).Include(l => l.Media).Where(l => l.Status == ListingStatus.Active && l.IsFeatured).OrderByDescending(l => l.Weight).Take(count).ToListAsync();

    public async Task<IEnumerable<Listing>> GetRecentAsync(int count)
        => await _dbSet.Include(l => l.Category).Include(l => l.Media).Where(l => l.Status == ListingStatus.Active).OrderByDescending(l => l.CreatedAt).Take(count).ToListAsync();

    public async Task IncrementViewCountAsync(Guid id)
    {
        await _dbSet.Where(l => l.Id == id).ExecuteUpdateAsync(s => s.SetProperty(l => l.ViewCount, l => l.ViewCount + 1));
    }
}
