import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { AdminDashboard } from '../models';

@Injectable({ providedIn: 'root' })
export class AdminDashboardService {
  constructor(private api: ApiService) {}

  getDashboard(): Observable<AdminDashboard> {
    return this.api.get<AdminDashboard>('admin/admindashboard');
  }
}
