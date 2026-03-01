import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ListingEngagement, PageViewStats, VisitorStats, LikedListing, FollowedListing } from '../models';

@Injectable({ providedIn: 'root' })
export class EngagementService {
  constructor(private api: ApiService) {}

  getEngagement(listingId: string): Observable<ListingEngagement> {
    return this.api.get<ListingEngagement>(`engagement/${listingId}`);
  }

  toggleLike(listingId: string): Observable<{ isLiked: boolean }> {
    return this.api.post<{ isLiked: boolean }>(`engagement/${listingId}/like`, {});
  }

  toggleFollow(listingId: string, notifyOnUpdate = true): Observable<{ isFollowing: boolean }> {
    return this.api.post<{ isFollowing: boolean }>(`engagement/${listingId}/follow?notifyOnUpdate=${notifyOnUpdate}`, {});
  }

  trackView(listingId: string): Observable<void> {
    return this.api.post<void>(`engagement/${listingId}/view`, {});
  }

  getPageViewStats(listingId: string, days = 30): Observable<PageViewStats> {
    return this.api.get<PageViewStats>(`engagement/${listingId}/views`, { days });
  }

  getVisitorStats(listingId: string, days = 30): Observable<VisitorStats> {
    return this.api.get<VisitorStats>(`engagement/${listingId}/visitors`, { days });
  }

  getLikedListings(): Observable<LikedListing[]> {
    return this.api.get<LikedListing[]>('engagement/liked');
  }

  getFollowedListings(): Observable<FollowedListing[]> {
    return this.api.get<FollowedListing[]>('engagement/followed');
  }
}
