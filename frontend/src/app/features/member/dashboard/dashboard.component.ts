import { Component, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe, PercentPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { KpiService } from '../../../core/services/kpi.service';
import { UserKpiDashboard, KpiTimeSeries, CategoryPerformance } from '../../../core/models';

@Component({
  selector: 'app-member-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule, DecimalPipe, PercentPipe, DatePipe],
  template: `
    <div class="dashboard">
      <div class="container">
        <!-- Header -->
        <div class="dash-header">
          <div>
            <h1 class="dash-header__title">{{ 'nav.dashboard' | translate }}</h1>
            <p class="dash-header__sub">Your performance overview</p>
          </div>
          <div class="dash-header__actions">
            <a routerLink="/member/listings/create" class="btn btn--primary">
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><path d="M12 5v14M5 12h14"/></svg>
              New Listing
            </a>
            <a routerLink="/member/messages" class="btn btn--outline">Messages</a>
          </div>
        </div>

        @if (dashboard) {
          <!-- KPI Summary Cards -->
          <div class="kpi-grid animate-in">
            <div class="kpi-card">
              <div class="kpi-card__icon kpi-card__icon--views">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/></svg>
              </div>
              <div class="kpi-card__data">
                <div class="kpi-card__value">{{ dashboard.summary.totalViews | number }}</div>
                <div class="kpi-card__label">Total Views</div>
              </div>
              <div class="kpi-card__trend" [class.kpi-card__trend--up]="dashboard.summary.viewsTrend > 0" [class.kpi-card__trend--down]="dashboard.summary.viewsTrend < 0">
                {{ dashboard.summary.viewsTrend > 0 ? '+' : '' }}{{ dashboard.summary.viewsTrend | number:'1.0-0' }}%
              </div>
            </div>

            <div class="kpi-card">
              <div class="kpi-card__icon kpi-card__icon--likes">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M20.84 4.61a5.5 5.5 0 00-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 00-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 000-7.78z"/></svg>
              </div>
              <div class="kpi-card__data">
                <div class="kpi-card__value">{{ dashboard.summary.totalLikes | number }}</div>
                <div class="kpi-card__label">Total Likes</div>
              </div>
              <div class="kpi-card__trend" [class.kpi-card__trend--up]="dashboard.summary.likesTrend > 0" [class.kpi-card__trend--down]="dashboard.summary.likesTrend < 0">
                {{ dashboard.summary.likesTrend > 0 ? '+' : '' }}{{ dashboard.summary.likesTrend | number:'1.0-0' }}%
              </div>
            </div>

            <div class="kpi-card">
              <div class="kpi-card__icon kpi-card__icon--followers">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 00-3-3.87M16 3.13a4 4 0 010 7.75"/></svg>
              </div>
              <div class="kpi-card__data">
                <div class="kpi-card__value">{{ dashboard.summary.totalFollowers | number }}</div>
                <div class="kpi-card__label">Followers</div>
              </div>
            </div>

            <div class="kpi-card">
              <div class="kpi-card__icon kpi-card__icon--messages">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M21 15a2 2 0 01-2 2H7l-4 4V5a2 2 0 012-2h14a2 2 0 012 2z"/></svg>
              </div>
              <div class="kpi-card__data">
                <div class="kpi-card__value">{{ dashboard.summary.totalMessages | number }}</div>
                <div class="kpi-card__label">Messages</div>
              </div>
              <div class="kpi-card__trend" [class.kpi-card__trend--up]="dashboard.summary.messagesTrend > 0" [class.kpi-card__trend--down]="dashboard.summary.messagesTrend < 0">
                {{ dashboard.summary.messagesTrend > 0 ? '+' : '' }}{{ dashboard.summary.messagesTrend | number:'1.0-0' }}%
              </div>
            </div>

            <div class="kpi-card">
              <div class="kpi-card__icon kpi-card__icon--rating">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor"><path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z"/></svg>
              </div>
              <div class="kpi-card__data">
                <div class="kpi-card__value">{{ dashboard.summary.averageRating | number:'1.1-1' }}</div>
                <div class="kpi-card__label">Avg Rating</div>
              </div>
            </div>

            <div class="kpi-card">
              <div class="kpi-card__icon kpi-card__icon--response">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><path d="M12 6v6l4 2"/></svg>
              </div>
              <div class="kpi-card__data">
                <div class="kpi-card__value">{{ dashboard.summary.responseRate | number:'1.0-0' }}%</div>
                <div class="kpi-card__label">Response Rate</div>
              </div>
            </div>
          </div>

          <!-- Views Chart -->
          <div class="dash-row animate-in animate-in-delay-2">
            <div class="chart-card card">
              <div class="card__header">
                <h3>Views Over Time</h3>
                <span class="chart-card__period">Last 30 days</span>
              </div>
              <div class="card__body">
                <div class="sparkline">
                  @for (point of dashboard.viewsOverTime; track point.date; let i = $index) {
                    <div class="sparkline__bar-wrap" [title]="point.date + ': ' + point.value">
                      <div class="sparkline__bar" [style.height.%]="getBarHeight(point.value, dashboard.viewsOverTime)">
                      </div>
                    </div>
                  }
                </div>
                <div class="sparkline__labels">
                  @if (dashboard.viewsOverTime.length) {
                    <span>{{ dashboard.viewsOverTime[0].date | date:'MMM d' }}</span>
                    <span>{{ dashboard.viewsOverTime[dashboard.viewsOverTime.length - 1].date | date:'MMM d' }}</span>
                  }
                </div>
              </div>
            </div>

            <!-- Quick Stats -->
            <div class="quick-stats card">
              <div class="card__header"><h3>Listing Stats</h3></div>
              <div class="card__body">
                <div class="quick-stat">
                  <span class="quick-stat__label">Active Listings</span>
                  <span class="quick-stat__value">{{ dashboard.summary.activeListings }}</span>
                </div>
                <div class="quick-stat">
                  <span class="quick-stat__label">Total Listings</span>
                  <span class="quick-stat__value">{{ dashboard.summary.totalListings }}</span>
                </div>
                <div class="quick-stat">
                  <span class="quick-stat__label">Total Revenue</span>
                  <span class="quick-stat__value quick-stat__value--accent">CHF {{ dashboard.summary.totalRevenue | number:'1.2-2' }}</span>
                </div>
                <div class="quick-stat">
                  <span class="quick-stat__label">Avg Response</span>
                  <span class="quick-stat__value">{{ dashboard.summary.averageResponseTime || '-' }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Category Performance -->
          @if (dashboard.categoryPerformance.length) {
            <div class="card animate-in animate-in-delay-3">
              <div class="card__header"><h3>Category Performance</h3></div>
              <div class="card__body" style="padding: 0;">
                <table class="table">
                  <thead>
                    <tr>
                      <th>Category</th>
                      <th>Listings</th>
                      <th>Views</th>
                      <th>Likes</th>
                      <th>Rating</th>
                    </tr>
                  </thead>
                  <tbody>
                    @for (cat of dashboard.categoryPerformance; track cat.categoryName) {
                      <tr>
                        <td><strong>{{ cat.categoryName }}</strong></td>
                        <td>{{ cat.listingCount }}</td>
                        <td>{{ cat.totalViews | number }}</td>
                        <td>{{ cat.totalLikes | number }}</td>
                        <td>
                          <div class="rating-inline">
                            <svg width="12" height="12" viewBox="0 0 24 24" fill="var(--color-warning)"><path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z"/></svg>
                            {{ cat.averageRating | number:'1.1-1' }}
                          </div>
                        </td>
                      </tr>
                    }
                  </tbody>
                </table>
              </div>
            </div>
          }

          <!-- Quick Actions -->
          <div class="quick-actions animate-in animate-in-delay-4">
            <a routerLink="/member/listings/create" class="quick-action">
              <div class="quick-action__icon">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 5v14M5 12h14"/></svg>
              </div>
              <span class="quick-action__text">Create Listing</span>
            </a>
            <a routerLink="/member/messages" class="quick-action">
              <div class="quick-action__icon">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M21 15a2 2 0 01-2 2H7l-4 4V5a2 2 0 012-2h14a2 2 0 012 2z"/></svg>
              </div>
              <span class="quick-action__text">View Messages</span>
            </a>
            <a routerLink="/member/listings" class="quick-action">
              <div class="quick-action__icon">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z"/><path d="M14 2v6h6M16 13H8M16 17H8M10 9H8"/></svg>
              </div>
              <span class="quick-action__text">My Listings</span>
            </a>
            <a routerLink="/member/boosts" class="quick-action">
              <div class="quick-action__icon">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M13 2L3 14h9l-1 8 10-12h-9l1-8z"/></svg>
              </div>
              <span class="quick-action__text">Manage Boosts</span>
            </a>
          </div>
        } @else {
          <!-- Loading -->
          <div class="kpi-grid">
            @for (i of [1,2,3,4,5,6]; track i) {
              <div class="kpi-card">
                <div class="skeleton" style="width: 40px; height: 40px; border-radius: 10px;"></div>
                <div class="kpi-card__data">
                  <div class="skeleton" style="width: 60px; height: 28px; margin-bottom: 4px;"></div>
                  <div class="skeleton" style="width: 80px; height: 14px;"></div>
                </div>
              </div>
            }
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    .dashboard {
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
        &__actions { width: 100%; }
      }
    }

    // ─── KPI Grid ────────────────────────────────────────────

    .kpi-grid {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: var(--space-4);
      margin-bottom: var(--space-6);

      @media (max-width: 960px) { grid-template-columns: repeat(2, 1fr); }
      @media (max-width: 480px) { grid-template-columns: 1fr; }
    }

    .kpi-card {
      display: flex;
      align-items: center;
      gap: var(--space-4);
      background: var(--color-surface-raised);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-lg);
      padding: var(--space-5);

      &__icon {
        width: 44px;
        height: 44px;
        border-radius: var(--radius-md);
        display: flex;
        align-items: center;
        justify-content: center;
        flex-shrink: 0;

        &--views { background: #eff6ff; color: #2563eb; }
        &--likes { background: #fef2f2; color: #dc2626; }
        &--followers { background: #f0fdf4; color: #16a34a; }
        &--messages { background: #fff7ed; color: #e8590c; }
        &--rating { background: #fffbeb; color: #d97706; }
        &--response { background: #faf5ff; color: #7c3aed; }
      }

      &__data { flex: 1; min-width: 0; }

      &__value {
        font-family: var(--font-display);
        font-size: var(--text-2xl);
        font-weight: 800;
        color: var(--color-ink);
        letter-spacing: -0.02em;
        line-height: 1;
      }

      &__label {
        font-family: var(--font-display);
        font-size: var(--text-xs);
        font-weight: 500;
        color: var(--color-ink-muted);
        margin-top: var(--space-1);
      }

      &__trend {
        font-family: var(--font-display);
        font-size: var(--text-xs);
        font-weight: 700;
        padding: var(--space-1) var(--space-2);
        border-radius: var(--radius-full);
        flex-shrink: 0;

        &--up {
          background: var(--color-success-bg);
          color: var(--color-success);
        }
        &--down {
          background: var(--color-danger-bg);
          color: var(--color-danger);
        }
      }
    }

    // ─── Dashboard Row ───────────────────────────────────────

    .dash-row {
      display: grid;
      grid-template-columns: 2fr 1fr;
      gap: var(--space-6);
      margin-bottom: var(--space-6);

      @media (max-width: 768px) {
        grid-template-columns: 1fr;
      }
    }

    .chart-card {
      .card__header {
        display: flex;
        align-items: center;
        justify-content: space-between;

        h3 {
          font-size: var(--text-base);
          font-weight: 600;
        }
      }

      &__period {
        font-size: var(--text-xs);
        color: var(--color-ink-faint);
      }
    }

    // ─── Sparkline Chart ─────────────────────────────────────

    .sparkline {
      display: flex;
      align-items: flex-end;
      gap: 2px;
      height: 140px;
      padding: var(--space-2) 0;

      &__bar-wrap {
        flex: 1;
        height: 100%;
        display: flex;
        align-items: flex-end;
        cursor: pointer;
      }

      &__bar {
        width: 100%;
        background: linear-gradient(to top, var(--color-accent), var(--color-accent-light));
        border-radius: 2px 2px 0 0;
        min-height: 2px;
        transition: opacity var(--duration-fast) var(--ease-out);
      }

      &__bar-wrap:hover &__bar {
        opacity: 0.7;
      }

      &__labels {
        display: flex;
        justify-content: space-between;
        margin-top: var(--space-2);
        font-size: var(--text-xs);
        color: var(--color-ink-faint);
      }
    }

    // ─── Quick Stats ────────────────────────────────────────

    .quick-stats {
      .card__header h3 {
        font-size: var(--text-base);
        font-weight: 600;
      }

      .card__body {
        display: flex;
        flex-direction: column;
        gap: var(--space-4);
      }
    }

    .quick-stat {
      display: flex;
      justify-content: space-between;
      align-items: center;

      &__label {
        font-size: var(--text-sm);
        color: var(--color-ink-muted);
      }

      &__value {
        font-family: var(--font-display);
        font-weight: 700;
        font-size: var(--text-sm);
        color: var(--color-ink);

        &--accent { color: var(--color-accent-dark); }
      }
    }

    // ─── Rating ──────────────────────────────────────────────

    .rating-inline {
      display: inline-flex;
      align-items: center;
      gap: 4px;
    }

    // ─── Category Table ──────────────────────────────────────

    .card { margin-bottom: var(--space-6); }

    .card__header {
      h3 { font-size: var(--text-base); font-weight: 600; }
    }

    // ─── Quick Actions ───────────────────────────────────────

    .quick-actions {
      display: grid;
      grid-template-columns: repeat(4, 1fr);
      gap: var(--space-4);

      @media (max-width: 768px) { grid-template-columns: repeat(2, 1fr); }
    }

    .quick-action {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: var(--space-3);
      padding: var(--space-6) var(--space-4);
      background: var(--color-surface-raised);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-lg);
      text-align: center;
      transition: all var(--duration-normal) var(--ease-out);

      &:hover {
        border-color: var(--color-accent);
        box-shadow: var(--shadow-md);
        transform: translateY(-2px);
      }

      &__icon {
        width: 48px;
        height: 48px;
        border-radius: var(--radius-md);
        background: var(--color-surface-sunken);
        color: var(--color-ink-light);
        display: flex;
        align-items: center;
        justify-content: center;
      }

      &:hover &__icon {
        background: var(--color-accent-bg);
        color: var(--color-accent);
      }

      &__text {
        font-family: var(--font-display);
        font-size: var(--text-sm);
        font-weight: 600;
        color: var(--color-ink);
      }
    }
  `]
})
export class MemberDashboardComponent implements OnInit {
  dashboard: UserKpiDashboard | null = null;

  constructor(private kpiService: KpiService) {}

  ngOnInit(): void {
    this.kpiService.getDashboard(30).subscribe(d => this.dashboard = d);
  }

  getBarHeight(value: number, series: KpiTimeSeries[]): number {
    const max = Math.max(...series.map(s => s.value), 1);
    return Math.max((value / max) * 100, 2);
  }
}
