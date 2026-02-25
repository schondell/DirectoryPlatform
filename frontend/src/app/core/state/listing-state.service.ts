import { Injectable, signal, computed } from '@angular/core';
import { Listing, PagedResult, ListingFilterRequest } from '../models';
import { ListingService } from '../services/listing.service';

@Injectable({ providedIn: 'root' })
export class ListingStateService {
  listings = signal<Listing[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  totalCount = signal(0);
  pageNumber = signal(1);
  pageSize = signal(20);
  filters = signal<ListingFilterRequest>({});
  attributeFilters = signal<Record<string, string>>({});

  totalPages = computed(() => Math.ceil(this.totalCount() / this.pageSize()));
  hasNextPage = computed(() => this.pageNumber() < this.totalPages());
  hasPreviousPage = computed(() => this.pageNumber() > 1);

  constructor(private listingService: ListingService) {}

  loadListings(): void {
    this.loading.set(true);
    this.error.set(null);

    const filter: ListingFilterRequest = {
      ...this.filters(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      attributes: this.attributeFilters()
    };

    this.listingService.getFiltered(filter).subscribe({
      next: (result: PagedResult<Listing>) => {
        this.listings.set(result.items);
        this.totalCount.set(result.totalCount);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.message || 'Failed to load listings');
        this.loading.set(false);
      }
    });
  }

  setFilters(filters: Partial<ListingFilterRequest>): void {
    this.filters.update(current => ({ ...current, ...filters }));
    this.pageNumber.set(1);
    this.loadListings();
  }

  setAttributeFilter(slug: string, value: string): void {
    this.attributeFilters.update(current => {
      const updated = { ...current };
      if (value) {
        updated[slug] = value;
      } else {
        delete updated[slug];
      }
      return updated;
    });
    this.pageNumber.set(1);
    this.loadListings();
  }

  clearAttributeFilters(): void {
    this.attributeFilters.set({});
    this.pageNumber.set(1);
    this.loadListings();
  }

  setPage(page: number): void {
    this.pageNumber.set(page);
    this.loadListings();
  }
}
