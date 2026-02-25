import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { ApiService } from '../../../core/services/api.service';
import { DashboardStats } from '../../../core/models';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  template: `
    <h1>{{ 'admin.dashboard' | translate }}</h1>
    <div class="stats-grid" *ngIf="stats">
      <div class="stat-card"><h3>{{ 'admin.totalUsers' | translate }}</h3><span>{{ stats.totalUsers }}</span></div>
      <div class="stat-card"><h3>{{ 'admin.totalListings' | translate }}</h3><span>{{ stats.totalListings }}</span></div>
      <div class="stat-card"><h3>{{ 'admin.activeListings' | translate }}</h3><span>{{ stats.activeListings }}</span></div>
      <div class="stat-card"><h3>{{ 'admin.pendingApproval' | translate }}</h3><span>{{ stats.pendingListings }}</span></div>
    </div>
  `
})
export class AdminDashboardComponent implements OnInit {
  stats: DashboardStats | null = null;
  constructor(private api: ApiService) {}
  ngOnInit(): void { this.api.get<DashboardStats>('admin/dashboard').subscribe(s => this.stats = s); }
}
