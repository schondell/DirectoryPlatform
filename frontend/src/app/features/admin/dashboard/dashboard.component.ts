import { Component, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { AdminDashboardService } from '../../../core/services/admin-dashboard.service';
import { AdminDashboard } from '../../../core/models';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule, DecimalPipe, DatePipe],
  template: `
    <div class="admin-dash">
      <div class="container">
        <!-- Header -->
        <div class="dash-header">
          <div>
            <h1 class="dash-header__title">{{ 'admin.dashboard' | translate }}</h1>
            <p class="dash-header__sub">System overview and administration</p>
          </div>
          <div class="dash-header__actions">
            <a routerLink="/admin/users" class="btn btn--outline btn--sm">Manage Users</a>
            <a routerLink="/admin/listings" class="btn btn--outline btn--sm">Manage Listings</a>
          </div>
        </div>

        @if (data) {
          <!-- Metric Cards -->
          <div class="metric-grid animate-in">
            <div class="metric-card metric-card--primary">
              <div class="metric-card__icon">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 00-3-3.87M16 3.13a4 4 0 010 7.75"/></svg>
              </div>
              <div class="metric-card__value">{{ data.overview.totalUsers | number }}</div>
              <div class="metric-card__label">{{ 'admin.totalUsers' | translate }}</div>
              <div class="metric-card__sub">+{{ data.overview.newUsersThisMonth }} this month</div>
            </div>

            <div class="metric-card">
              <div class="metric-card__icon">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z"/><path d="M14 2v6h6M16 13H8M16 17H8M10 9H8"/></svg>
              </div>
              <div class="metric-card__value">{{ data.overview.totalListings | number }}</div>
              <div class="metric-card__label">{{ 'admin.totalListings' | translate }}</div>
              <div class="metric-card__sub">+{{ data.overview.newListingsThisMonth }} this month</div>
            </div>

            <div class="metric-card metric-card--success">
              <div class="metric-card__icon">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 11.08V12a10 10 0 11-5.93-9.14"/><path d="M22 4L12 14.01l-3-3"/></svg>
              </div>
              <div class="metric-card__value">{{ data.overview.activeListings | number }}</div>
              <div class="metric-card__label">{{ 'admin.activeListings' | translate }}</div>
            </div>

            <div class="metric-card" [class.metric-card--warning]="data.overview.pendingApprovals > 0">
              <div class="metric-card__icon">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><path d="M12 6v6l4 2"/></svg>
              </div>
              <div class="metric-card__value">{{ data.overview.pendingApprovals }}</div>
              <div class="metric-card__label">{{ 'admin.pendingApproval' | translate }}</div>
            </div>

            <div class="metric-card metric-card--accent">
              <div class="metric-card__icon">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="12" y1="1" x2="12" y2="23"/><path d="M17 5H9.5a3.5 3.5 0 000 7h5a3.5 3.5 0 010 7H6"/></svg>
              </div>
              <div class="metric-card__value">CHF {{ data.overview.monthlyRevenue | number:'1.0-0' }}</div>
              <div class="metric-card__label">Monthly Revenue</div>
            </div>
          </div>

          <!-- Tables Row -->
          <div class="tables-row animate-in animate-in-delay-2">
            <!-- Recent Users -->
            <div class="card">
              <div class="card__header">
                <h3>Recent Users</h3>
                <a routerLink="/admin/users" class="table-link">View all</a>
              </div>
              <div class="card__body" style="padding: 0;">
                <table class="table">
                  <thead>
                    <tr>
                      <th>Username</th>
                      <th>Email</th>
                      <th>Joined</th>
                    </tr>
                  </thead>
                  <tbody>
                    @for (user of data.recentActivity.recentUsers; track user.id) {
                      <tr>
                        <td>
                          <div class="user-cell">
                            <div class="user-cell__avatar">{{ user.username.charAt(0).toUpperCase() }}</div>
                            <strong>{{ user.username }}</strong>
                          </div>
                        </td>
                        <td>{{ user.email }}</td>
                        <td>{{ user.createdAt | date:'mediumDate' }}</td>
                      </tr>
                    }
                    @if (!data.recentActivity.recentUsers.length) {
                      <tr><td colspan="3" class="empty-state">No recent users</td></tr>
                    }
                  </tbody>
                </table>
              </div>
            </div>

            <!-- Recent Listings -->
            <div class="card">
              <div class="card__header">
                <h3>Recent Listings</h3>
                <a routerLink="/admin/listings" class="table-link">View all</a>
              </div>
              <div class="card__body" style="padding: 0;">
                <table class="table">
                  <thead>
                    <tr>
                      <th>Title</th>
                      <th>Category</th>
                      <th>Status</th>
                      <th>Created</th>
                    </tr>
                  </thead>
                  <tbody>
                    @for (listing of data.recentActivity.recentListings; track listing.id) {
                      <tr>
                        <td><strong>{{ listing.title }}</strong></td>
                        <td>{{ listing.categoryName || '-' }}</td>
                        <td>
                          <span class="badge" [class]="getStatusBadge(listing.status)">{{ listing.status }}</span>
                        </td>
                        <td>{{ listing.createdAt | date:'mediumDate' }}</td>
                      </tr>
                    }
                    @if (!data.recentActivity.recentListings.length) {
                      <tr><td colspan="4" class="empty-state">No recent listings</td></tr>
                    }
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          <!-- System Health -->
          <div class="card animate-in animate-in-delay-3">
            <div class="card__header"><h3>System Health</h3></div>
            <div class="card__body">
              <div class="health-grid">
                <div class="health-item">
                  <div class="health-item__label">CPU Usage</div>
                  <div class="health-item__bar">
                    <div class="health-item__fill" [style.width.%]="data.systemHealth.cpuUsagePercent" [class.health-item__fill--warning]="data.systemHealth.cpuUsagePercent > 70" [class.health-item__fill--danger]="data.systemHealth.cpuUsagePercent > 90"></div>
                  </div>
                  <div class="health-item__value">{{ data.systemHealth.cpuUsagePercent | number:'1.0-0' }}%</div>
                </div>

                <div class="health-item">
                  <div class="health-item__label">Memory</div>
                  <div class="health-item__bar">
                    <div class="health-item__fill" [style.width.%]="(data.systemHealth.memoryUsageMB / data.systemHealth.memoryTotalMB) * 100" [class.health-item__fill--warning]="data.systemHealth.memoryUsageMB / data.systemHealth.memoryTotalMB > 0.7"></div>
                  </div>
                  <div class="health-item__value">{{ data.systemHealth.memoryUsageMB | number:'1.0-0' }} / {{ data.systemHealth.memoryTotalMB | number:'1.0-0' }} MB</div>
                </div>

                <div class="health-item">
                  <div class="health-item__label">.NET Version</div>
                  <div class="health-item__text">{{ data.systemHealth.dotNetVersion }}</div>
                </div>

                <div class="health-item">
                  <div class="health-item__label">Platform</div>
                  <div class="health-item__text">{{ data.systemHealth.osPlatform }}</div>
                </div>

                <div class="health-item">
                  <div class="health-item__label">Uptime</div>
                  <div class="health-item__text">{{ data.systemHealth.uptime }}</div>
                </div>
              </div>
            </div>
          </div>
        } @else {
          <!-- Loading -->
          <div class="metric-grid">
            @for (i of [1,2,3,4,5]; track i) {
              <div class="metric-card">
                <div class="skeleton" style="width: 40px; height: 40px; border-radius: 10px;"></div>
                <div class="skeleton" style="width: 80px; height: 32px; margin-top: 12px;"></div>
                <div class="skeleton" style="width: 100px; height: 14px; margin-top: 8px;"></div>
              </div>
            }
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    .admin-dash {
      padding: var(--space-8) 0 var(--space-16);
    }

    .dash-header {
      display: flex;
      align-items: flex-start;
      justify-content: space-between;
      margin-bottom: var(--space-8);
      gap: var(--space-4);

      &__title {
        font-size: var(--text-2xl);
        font-weight: 700;
      }

      &__sub {
        font-size: var(--text-sm);
        color: var(--color-ink-muted);
        margin-top: var(--space-1);
      }

      &__actions {
        display: flex;
        gap: var(--space-3);
      }

      @media (max-width: 640px) {
        flex-direction: column;
      }
    }

    // ─── Metric Grid ────────────────────────────────────────

    .metric-grid {
      display: grid;
      grid-template-columns: repeat(5, 1fr);
      gap: var(--space-4);
      margin-bottom: var(--space-8);

      @media (max-width: 1024px) { grid-template-columns: repeat(3, 1fr); }
      @media (max-width: 640px) { grid-template-columns: repeat(2, 1fr); }
      @media (max-width: 400px) { grid-template-columns: 1fr; }
    }

    .metric-card {
      background: var(--color-surface-raised);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-lg);
      padding: var(--space-5);
      position: relative;
      overflow: hidden;

      &::before {
        content: '';
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        height: 3px;
        background: var(--color-border-strong);
        border-radius: var(--radius-lg) var(--radius-lg) 0 0;
      }

      &--primary::before { background: var(--color-info); }
      &--success::before { background: var(--color-success); }
      &--warning::before { background: var(--color-warning); }
      &--accent::before { background: var(--color-accent); }

      &__icon {
        color: var(--color-ink-muted);
        margin-bottom: var(--space-3);
      }

      &__value {
        font-family: var(--font-display);
        font-size: var(--text-2xl);
        font-weight: 800;
        color: var(--color-ink);
        letter-spacing: -0.02em;
      }

      &__label {
        font-family: var(--font-display);
        font-size: var(--text-xs);
        font-weight: 600;
        text-transform: uppercase;
        letter-spacing: 0.04em;
        color: var(--color-ink-muted);
        margin-top: var(--space-1);
      }

      &__sub {
        font-size: var(--text-xs);
        color: var(--color-ink-faint);
        margin-top: var(--space-2);
      }
    }

    // ─── Tables Row ──────────────────────────────────────────

    .tables-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--space-6);
      margin-bottom: var(--space-6);

      @media (max-width: 768px) {
        grid-template-columns: 1fr;
      }
    }

    .card__header {
      display: flex;
      align-items: center;
      justify-content: space-between;

      h3 {
        font-size: var(--text-base);
        font-weight: 600;
      }
    }

    .table-link {
      font-family: var(--font-display);
      font-size: var(--text-xs);
      font-weight: 600;
      color: var(--color-accent);
      &:hover { color: var(--color-accent-dark); }
    }

    .user-cell {
      display: flex;
      align-items: center;
      gap: var(--space-2);

      &__avatar {
        width: 28px;
        height: 28px;
        border-radius: var(--radius-full);
        background: var(--color-primary);
        color: white;
        display: flex;
        align-items: center;
        justify-content: center;
        font-family: var(--font-display);
        font-weight: 700;
        font-size: 0.625rem;
        flex-shrink: 0;
      }
    }

    .empty-state {
      text-align: center;
      color: var(--color-ink-faint);
      font-style: italic;
      padding: var(--space-8) !important;
    }

    // ─── System Health ────────────────────────────────────────

    .health-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
      gap: var(--space-6);
    }

    .health-item {
      &__label {
        font-family: var(--font-display);
        font-size: var(--text-xs);
        font-weight: 600;
        text-transform: uppercase;
        letter-spacing: 0.04em;
        color: var(--color-ink-muted);
        margin-bottom: var(--space-2);
      }

      &__bar {
        height: 8px;
        background: var(--color-surface-sunken);
        border-radius: var(--radius-full);
        overflow: hidden;
        margin-bottom: var(--space-1);
      }

      &__fill {
        height: 100%;
        background: var(--color-success);
        border-radius: var(--radius-full);
        transition: width var(--duration-slow) var(--ease-out);

        &--warning { background: var(--color-warning); }
        &--danger { background: var(--color-danger); }
      }

      &__value {
        font-family: var(--font-display);
        font-size: var(--text-sm);
        font-weight: 600;
        color: var(--color-ink);
      }

      &__text {
        font-family: var(--font-mono);
        font-size: var(--text-sm);
        color: var(--color-ink-light);
      }
    }
  `]
})
export class AdminDashboardComponent implements OnInit {
  data: AdminDashboard | null = null;

  constructor(private adminDashboardService: AdminDashboardService) {}

  ngOnInit(): void {
    this.adminDashboardService.getDashboard().subscribe(d => this.data = d);
  }

  getStatusBadge(status: string): string {
    switch (status.toLowerCase()) {
      case 'active': return 'badge--success';
      case 'pending': return 'badge--warning';
      case 'rejected': return 'badge--danger';
      case 'expired': return 'badge--neutral';
      default: return 'badge--neutral';
    }
  }
}
