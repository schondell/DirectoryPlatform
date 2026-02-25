import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Subscription, SubscriptionTier } from '../models';

@Injectable({ providedIn: 'root' })
export class SubscriptionService {
  constructor(private api: ApiService) {}

  getActive(): Observable<Subscription> {
    return this.api.get<Subscription>('subscriptions/my');
  }

  getTiers(): Observable<SubscriptionTier[]> {
    return this.api.get<SubscriptionTier[]>('subscriptions/tiers');
  }

  create(dto: any): Observable<Subscription> {
    return this.api.post<Subscription>('subscriptions', dto);
  }

  cancel(id: string): Observable<void> {
    return this.api.put<void>(`subscriptions/${id}/cancel`, {});
  }
}
