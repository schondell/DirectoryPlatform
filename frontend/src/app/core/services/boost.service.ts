import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Boost, BoostPricing, CreateBoost } from '../models';

@Injectable({ providedIn: 'root' })
export class BoostService {
  constructor(private api: ApiService) {}

  getPricing(): Observable<BoostPricing[]> {
    return this.api.get<BoostPricing[]>('boost/pricing');
  }

  createBoost(dto: CreateBoost): Observable<Boost> {
    return this.api.post<Boost>('boost', dto);
  }

  getActiveBoosts(listingId: string): Observable<Boost[]> {
    return this.api.get<Boost[]>(`boost/listing/${listingId}`);
  }

  getMyBoosts(): Observable<Boost[]> {
    return this.api.get<Boost[]>('boost/my');
  }
}
