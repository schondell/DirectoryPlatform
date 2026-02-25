import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Listing, ListingFilterRequest, PagedResult } from '../models';

@Injectable({ providedIn: 'root' })
export class ListingService {
  constructor(private api: ApiService) {}

  getFiltered(filter: ListingFilterRequest): Observable<PagedResult<Listing>> {
    const params: Record<string, any> = {
      pageNumber: filter.pageNumber ?? 1,
      pageSize: filter.pageSize ?? 20,
    };
    if (filter.searchTerm) params['search'] = filter.searchTerm;
    if (filter.categoryId) params['categoryId'] = filter.categoryId;
    if (filter.regionId) params['regionId'] = filter.regionId;
    if (filter.sortBy) params['sortBy'] = filter.sortBy;
    if (filter.ascending !== undefined) params['ascending'] = filter.ascending;
    if (filter.attributes) {
      Object.entries(filter.attributes).forEach(([key, value]) => {
        params[`attr[${key}]`] = value;
      });
    }
    return this.api.get<PagedResult<Listing>>('listings', params);
  }

  getById(id: string): Observable<Listing> {
    return this.api.get<Listing>(`listings/${id}`);
  }

  getFeatured(count = 10): Observable<Listing[]> {
    return this.api.get<Listing[]>('listings/featured', { count });
  }

  getRecent(count = 10): Observable<Listing[]> {
    return this.api.get<Listing[]>('listings/recent', { count });
  }

  getMyListings(): Observable<Listing[]> {
    return this.api.get<Listing[]>('listings/my');
  }

  create(dto: any): Observable<Listing> {
    return this.api.post<Listing>('listings', dto);
  }

  update(id: string, dto: any): Observable<Listing> {
    return this.api.put<Listing>(`listings/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`listings/${id}`);
  }
}
