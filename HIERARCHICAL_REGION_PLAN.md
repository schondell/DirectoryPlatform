# Hierarchical Canton → Town Filtering for DirectoryPlatform

## Context
DirectoryPlatform already has `GetRegionAndChildIds()` in `ListingRepository.cs` (line 163) that filters listings by region subtree. What's missing:
1. Listing count aggregation per region (so parent regions show child counts)
2. Frontend cascading Canton + Town dropdowns (currently a single flat indented select)

## Changes

### 1. Backend: Add `GetRegionsWithListingsCountAsync` to RegionRepository
**File:** `backend/DirectoryPlatform/src/Infrastructure/Repositories/RegionRepository.cs`

Add a new method that:
- Loads all regions in one query
- Runs a single GroupBy query on Listings to get direct counts per RegionId (where Status == Active)
- Walks the hierarchy in-memory: parent count = own listings + sum of child counts
- Returns `IDictionary<Region, int>`

Also add to the interface:
**File:** `backend/DirectoryPlatform/src/Core/Interfaces/IRegionRepository.cs`
- Add: `Task<IDictionary<Region, int>> GetRegionsWithListingsCountAsync();`

Reference implementation (from DirectoryWebsite):
```csharp
public async Task<IDictionary<Region, int>> GetRegionsWithListingsCountAsync()
{
    var regions = await _dbSet.ToListAsync();

    var directCounts = await _context.Listings
        .Where(l => l.Status == ListingStatus.Active)
        .GroupBy(l => l.RegionId)
        .Select(g => new { RegionId = g.Key, Count = g.Count() })
        .ToDictionaryAsync(x => x.RegionId, x => x.Count);

    var countByRegion = new Dictionary<Guid, int>();
    foreach (var region in regions)
    {
        directCounts.TryGetValue(region.Id, out var direct);
        countByRegion[region.Id] = direct;
    }

    // Aggregate: parent count += child count
    var regionLookup = regions.ToDictionary(r => r.Id);
    foreach (var region in regions)
    {
        if (region.ParentId.HasValue && regionLookup.ContainsKey(region.ParentId.Value))
        {
            countByRegion[region.ParentId.Value] += countByRegion[region.Id];
        }
    }

    var result = new Dictionary<Region, int>();
    foreach (var region in regions)
        result.Add(region, countByRegion[region.Id]);
    return result;
}
```

### 2. Backend: Expose via API endpoint
Add a `GET /api/regions/with-listings-count` endpoint in the RegionsController that calls the new method and returns `RegionWithCountDto[]` (id, name, slug, parentId, displayOrder, listingCount).

### 3. Frontend: Add `RegionWithCount` model
**File:** `frontend/src/app/core/models/index.ts`

Add after existing `RegionWithChildren`:
```typescript
export interface RegionWithCount extends Region {
  listingCount: number;
}
```

### 4. Frontend: Add service method
**File:** `frontend/src/app/core/services/region.service.ts`

Add:
```typescript
getWithListingsCount(): Observable<RegionWithCount[]> {
  return this.api.get<RegionWithCount[]>('regions/with-listings-count');
}

getChildren(id: string): Observable<Region[]> {
  return this.api.get<Region[]>(`regions/${id}/children`);
}
```

### 5. Frontend: Update listing-grid component for cascading dropdowns
**File:** `frontend/src/app/features/public/listing-grid/listing-grid.component.ts`

Replace the single region dropdown (lines 33-44) with two cascading selects:

```typescript
// New properties
allRegions: RegionWithCount[] = [];
cantons: RegionWithCount[] = [];
towns: Region[] = [];
selectedCantonId = '';

// In ngOnInit, replace regionService.getTree() with:
this.regionService.getWithListingsCount().subscribe(regions => {
  this.allRegions = regions;
  const root = regions.find(r => !r.parentId);
  this.cantons = root
    ? regions.filter(r => r.parentId === root.id)
    : [];
});
```

Replace region select in the inline template with:
```html
<div class="filter-group">
  <label>{{ 'listing.canton' | translate }}</label>
  <select [(ngModel)]="selectedCantonId" (ngModelChange)="onCantonChange($event)">
    <option value="">{{ 'listing.allCantons' | translate }}</option>
    @for (canton of cantons; track canton.id) {
      <option [value]="canton.id">{{ canton.name }} ({{ canton.listingCount }})</option>
    }
  </select>
</div>

@if (towns.length > 0) {
  <div class="filter-group">
    <label>{{ 'listing.town' | translate }}</label>
    <select [(ngModel)]="selectedRegionId" (ngModelChange)="onRegionChange($event)">
      <option value="">{{ 'listing.allTowns' | translate }}</option>
      @for (town of towns; track town.id) {
        <option [value]="town.id">{{ town.name }}</option>
      }
    </select>
  </div>
}
```

Add handler methods:
```typescript
onCantonChange(cantonId: string): void {
  this.selectedRegionId = '';
  if (cantonId) {
    this.regionService.getChildren(cantonId).subscribe(t => this.towns = t);
    this.state.setFilters({ regionId: cantonId });
  } else {
    this.towns = [];
    this.state.setFilters({ regionId: undefined });
  }
}
```

Keep existing `onRegionChange` — it already calls `state.setFilters({ regionId })`, which the backend already handles via `GetRegionAndChildIds`.

### 6. Frontend: Translation keys
Add to all i18n files (en, de, fr, es, it):
- `listing.canton` / `listing.allCantons` / `listing.town` / `listing.allTowns`

## Key Points
- Backend region subtree filtering **already works** via `GetRegionAndChildIds()` in ListingRepository
- Only missing piece is listing count aggregation + API endpoint + frontend cascading UX
- The platform uses signals (not RxJS subjects), `inject()` pattern, and inline templates
- `ngModel` two-way binding (not ReactiveFormsModule)

## Verification
1. Build backend: `dotnet build`
2. Build frontend: `npx tsc --noEmit`
3. Select a parent region → town dropdown appears → selecting a town further filters
4. Parent region cards/dropdown show aggregated child listing counts
