import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  ScraperAnalyticsDashboard,
  ScraperOverview,
  LifecycleEntry,
  VelocityEntry,
  DealerEntry,
  RepostEntry,
  PaidVsFreeEntry,
  PriceDistributionEntry,
  GeoEntry,
  ScraperFreshness
} from '../models';

@Injectable({ providedIn: 'root' })
export class ScraperAnalyticsService {
  private basePath = 'admin/scraper-analytics';

  constructor(private api: ApiService) {}

  getDashboard(): Observable<ScraperAnalyticsDashboard> {
    return this.api.get<ScraperAnalyticsDashboard>(this.basePath);
  }

  getOverview(): Observable<ScraperOverview> {
    return this.api.get<ScraperOverview>(`${this.basePath}/overview`);
  }

  getLifecycle(): Observable<LifecycleEntry[]> {
    return this.api.get<LifecycleEntry[]>(`${this.basePath}/lifecycle`);
  }

  getVelocity(): Observable<VelocityEntry[]> {
    return this.api.get<VelocityEntry[]>(`${this.basePath}/velocity`);
  }

  getDealers(): Observable<DealerEntry[]> {
    return this.api.get<DealerEntry[]>(`${this.basePath}/dealers`);
  }

  getReposts(): Observable<RepostEntry[]> {
    return this.api.get<RepostEntry[]>(`${this.basePath}/reposts`);
  }

  getPaidVsFree(): Observable<PaidVsFreeEntry[]> {
    return this.api.get<PaidVsFreeEntry[]>(`${this.basePath}/paid-vs-free`);
  }

  getPriceDistribution(): Observable<PriceDistributionEntry[]> {
    return this.api.get<PriceDistributionEntry[]>(`${this.basePath}/price-distribution`);
  }

  getGeographic(): Observable<GeoEntry[]> {
    return this.api.get<GeoEntry[]>(`${this.basePath}/geographic`);
  }

  getFreshness(): Observable<ScraperFreshness> {
    return this.api.get<ScraperFreshness>(`${this.basePath}/freshness`);
  }
}
