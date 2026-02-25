import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Review } from '../models';

@Injectable({ providedIn: 'root' })
export class ReviewService {
  constructor(private api: ApiService) {}

  getByListing(listingId: string): Observable<Review[]> {
    return this.api.get<Review[]>(`reviews/listing/${listingId}`);
  }

  getAverage(listingId: string): Observable<number> {
    return this.api.get<number>(`reviews/listing/${listingId}/average`);
  }

  create(dto: any): Observable<Review> {
    return this.api.post<Review>('reviews', dto);
  }

  updateStatus(id: string, status: string): Observable<Review> {
    return this.api.put<Review>(`reviews/${id}/status`, JSON.stringify(status));
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`reviews/${id}`);
  }
}
