import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { UserKpiDashboard, KpiSummary } from '../models';

@Injectable({ providedIn: 'root' })
export class KpiService {
  constructor(private api: ApiService) {}

  getDashboard(days = 30): Observable<UserKpiDashboard> {
    return this.api.get<UserKpiDashboard>('kpi/dashboard', { days });
  }

  getSummary(): Observable<KpiSummary> {
    return this.api.get<KpiSummary>('kpi/summary');
  }
}
