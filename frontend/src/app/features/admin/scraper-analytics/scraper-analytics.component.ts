import { Component, OnInit, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule, DecimalPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ScraperAnalyticsService } from '../../../core/services/scraper-analytics.service';
import { ScraperAnalyticsDashboard } from '../../../core/models';

@Component({
  selector: 'app-scraper-analytics',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, RouterLink, DecimalPipe, DatePipe],
  template: `
    <div class="scraper-dash">
      <div class="container">
        <!-- Header -->
        <div class="dash-header">
          <div>
            <h1 class="dash-header__title">Scraper Analytics</h1>
            <p class="dash-header__sub">petitesannonces.ch scraped data insights</p>
          </div>
          <div class="dash-header__actions">
            <a routerLink="/admin/dashboard" class="btn btn--outline btn--sm">Admin Dashboard</a>
          </div>
        </div>

        @if (data) {
          <!-- Overview Metric Cards -->
          <div class="metric-grid animate-in">
            <div class="metric-card metric-card--primary">
              <div class="metric-card__value">{{ data.overview.total | number }}</div>
              <div class="metric-card__label">Total Listings</div>
            </div>
            <div class="metric-card metric-card--success">
              <div class="metric-card__value">{{ data.overview.active | number }}</div>
              <div class="metric-card__label">Active</div>
            </div>
            <div class="metric-card metric-card--warning">
              <div class="metric-card__value">{{ data.overview.expired | number }}</div>
              <div class="metric-card__label">Expired</div>
            </div>
            <div class="metric-card metric-card--accent">
              <div class="metric-card__value">{{ data.overview.paid | number }}</div>
              <div class="metric-card__label">Paid</div>
            </div>
            <div class="metric-card">
              <div class="metric-card__value">{{ data.overview.withPhone | number }}</div>
              <div class="metric-card__label">With Phone</div>
            </div>
            <div class="metric-card">
              <div class="metric-card__value">{{ data.overview.withImages | number }}</div>
              <div class="metric-card__label">With Images</div>
            </div>
            <div class="metric-card">
              <div class="metric-card__value">{{ data.overview.uniquePhones | number }}</div>
              <div class="metric-card__label">Unique Sellers</div>
            </div>
            <div class="metric-card">
              <div class="metric-card__value">{{ data.overview.categories | number }}</div>
              <div class="metric-card__label">Categories</div>
            </div>
          </div>

          <!-- Freshness -->
          <div class="card animate-in animate-in-delay-2">
            <div class="card__header"><h3>Data Freshness</h3></div>
            <div class="card__body">
              <div class="freshness-grid">
                <div class="freshness-item">
                  <div class="freshness-item__label">Seen &lt; 24h</div>
                  <div class="freshness-item__bar">
                    <div class="freshness-item__fill freshness-item__fill--good"
                      [style.width.%]="data.freshness.totalActive ? (data.freshness.seen24h / data.freshness.totalActive * 100) : 0"></div>
                  </div>
                  <div class="freshness-item__value">{{ data.freshness.seen24h | number }}</div>
                </div>
                <div class="freshness-item">
                  <div class="freshness-item__label">Seen &lt; 7d</div>
                  <div class="freshness-item__bar">
                    <div class="freshness-item__fill freshness-item__fill--ok"
                      [style.width.%]="data.freshness.totalActive ? (data.freshness.seen7d / data.freshness.totalActive * 100) : 0"></div>
                  </div>
                  <div class="freshness-item__value">{{ data.freshness.seen7d | number }}</div>
                </div>
                <div class="freshness-item">
                  <div class="freshness-item__label">Seen &lt; 30d</div>
                  <div class="freshness-item__bar">
                    <div class="freshness-item__fill freshness-item__fill--stale"
                      [style.width.%]="data.freshness.totalActive ? (data.freshness.seen30d / data.freshness.totalActive * 100) : 0"></div>
                  </div>
                  <div class="freshness-item__value">{{ data.freshness.seen30d | number }}</div>
                </div>
                <div class="freshness-item">
                  <div class="freshness-item__label">Stale (&gt;30d)</div>
                  <div class="freshness-item__bar">
                    <div class="freshness-item__fill freshness-item__fill--danger"
                      [style.width.%]="data.freshness.totalActive ? (data.freshness.stale30d / data.freshness.totalActive * 100) : 0"></div>
                  </div>
                  <div class="freshness-item__value">{{ data.freshness.stale30d | number }}</div>
                </div>
              </div>
              <div class="freshness-avg">Avg days since seen: <strong>{{ data.freshness.avgDaysSinceSeen | number:'1.1-1' }}</strong></div>
            </div>
          </div>

          <!-- Velocity -->
          <div class="card animate-in animate-in-delay-3">
            <div class="card__header"><h3>Category Velocity</h3></div>
            <div class="card__body table-scroll">
              <table class="table">
                <thead>
                  <tr>
                    <th>Category</th>
                    <th class="text-right">Total</th>
                    <th class="text-right">Last 7d</th>
                    <th class="text-right">Last 24h</th>
                    <th class="text-right">Avg/Day</th>
                  </tr>
                </thead>
                <tbody>
                  @for (v of data.velocity; track v.category) {
                    <tr>
                      <td><strong>{{ v.category }}</strong></td>
                      <td class="text-right">{{ v.totalListings | number }}</td>
                      <td class="text-right">{{ v.last7d | number }}</td>
                      <td class="text-right">{{ v.last24h | number }}</td>
                      <td class="text-right">{{ v.avgPerDay | number:'1.1-1' }}</td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          </div>

          <!-- Lifecycle -->
          <div class="card animate-in">
            <div class="card__header"><h3>Listing Lifecycle (Expired)</h3></div>
            <div class="card__body table-scroll">
              <table class="table">
                <thead>
                  <tr>
                    <th>Category</th>
                    <th class="text-right">Expired</th>
                    <th class="text-right">Avg Days</th>
                    <th class="text-right">Median</th>
                    <th class="text-right">Min</th>
                    <th class="text-right">Max</th>
                  </tr>
                </thead>
                <tbody>
                  @for (l of data.lifecycle; track l.category) {
                    <tr>
                      <td><strong>{{ l.category }}</strong></td>
                      <td class="text-right">{{ l.expiredCount | number }}</td>
                      <td class="text-right">{{ l.avgDaysAlive | number:'1.1-1' }}</td>
                      <td class="text-right">{{ l.medianDays | number:'1.1-1' }}</td>
                      <td class="text-right">{{ l.minDays | number:'1.1-1' }}</td>
                      <td class="text-right">{{ l.maxDays | number:'1.1-1' }}</td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          </div>

          <!-- Two-column row: Dealers + Reposts -->
          <div class="tables-row animate-in">
            <!-- Dealers -->
            <div class="card">
              <div class="card__header"><h3>Dealer Detection (3+ listings)</h3></div>
              <div class="card__body table-scroll">
                <table class="table">
                  <thead>
                    <tr>
                      <th>Phone</th>
                      <th class="text-right">Listings</th>
                      <th class="text-right">Paid</th>
                      <th class="text-right">Active</th>
                      <th>Pseudos</th>
                    </tr>
                  </thead>
                  <tbody>
                    @for (d of data.dealers; track d.phone) {
                      <tr>
                        <td class="mono">{{ d.phone }}</td>
                        <td class="text-right">{{ d.listingCount }}</td>
                        <td class="text-right">{{ d.paidCount }}</td>
                        <td class="text-right">{{ d.activeCount }}</td>
                        <td>{{ d.pseudos.join(', ') || '-' }}</td>
                      </tr>
                    }
                  </tbody>
                </table>
              </div>
            </div>

            <!-- Reposts -->
            <div class="card">
              <div class="card__header"><h3>Repost Detection</h3></div>
              <div class="card__body table-scroll">
                <table class="table">
                  <thead>
                    <tr>
                      <th>Phone</th>
                      <th class="text-right">Reposts</th>
                      <th class="text-right">Avg Hrs</th>
                      <th>Sample Title</th>
                    </tr>
                  </thead>
                  <tbody>
                    @for (r of data.reposts; track r.phone + r.category) {
                      <tr>
                        <td class="mono">{{ r.phone }}</td>
                        <td class="text-right">{{ r.repostCount }}</td>
                        <td class="text-right">{{ r.avgHoursBetween != null ? (r.avgHoursBetween | number:'1.1-1') : '-' }}</td>
                        <td class="truncate">{{ r.sampleTitle || '-' }}</td>
                      </tr>
                    }
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          <!-- Two-column row: Paid vs Free + Prices -->
          <div class="tables-row animate-in">
            <!-- Paid vs Free -->
            <div class="card">
              <div class="card__header"><h3>Paid vs Free</h3></div>
              <div class="card__body table-scroll">
                <table class="table">
                  <thead>
                    <tr>
                      <th>Type</th>
                      <th class="text-right">Count</th>
                      <th class="text-right">Active</th>
                      <th class="text-right">Expired</th>
                      <th class="text-right">Avg Days</th>
                      <th class="text-right">Avg Imgs</th>
                    </tr>
                  </thead>
                  <tbody>
                    @for (p of data.paidVsFree; track p.isPaid + '' + p.paidType) {
                      <tr>
                        <td>
                          @if (p.isPaid) {
                            <span class="badge badge--success">{{ p.paidType || 'Paid' }}</span>
                          } @else {
                            <span class="badge badge--neutral">Free</span>
                          }
                        </td>
                        <td class="text-right">{{ p.count | number }}</td>
                        <td class="text-right">{{ p.active | number }}</td>
                        <td class="text-right">{{ p.expired | number }}</td>
                        <td class="text-right">{{ p.avgDaysAlive != null ? (p.avgDaysAlive | number:'1.1-1') : '-' }}</td>
                        <td class="text-right">{{ p.avgImages | number:'1.1-1' }}</td>
                      </tr>
                    }
                  </tbody>
                </table>
              </div>
            </div>

            <!-- Price Distribution -->
            <div class="card">
              <div class="card__header"><h3>Price Distribution (CHF)</h3></div>
              <div class="card__body table-scroll">
                <table class="table">
                  <thead>
                    <tr>
                      <th>Category</th>
                      <th class="text-right">Count</th>
                      <th class="text-right">Avg</th>
                      <th class="text-right">Median</th>
                      <th class="text-right">Min</th>
                      <th class="text-right">Max</th>
                    </tr>
                  </thead>
                  <tbody>
                    @for (p of data.priceDistribution; track p.category) {
                      <tr>
                        <td><strong>{{ p.category }}</strong></td>
                        <td class="text-right">{{ p.count | number }}</td>
                        <td class="text-right">{{ p.avg | number:'1.0-0' }}</td>
                        <td class="text-right">{{ p.median | number:'1.0-0' }}</td>
                        <td class="text-right">{{ p.min | number:'1.0-0' }}</td>
                        <td class="text-right">{{ p.max | number:'1.0-0' }}</td>
                      </tr>
                    }
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          <!-- Geographic -->
          <div class="card animate-in">
            <div class="card__header"><h3>Geographic Distribution</h3></div>
            <div class="card__body table-scroll">
              <table class="table">
                <thead>
                  <tr>
                    <th>Location</th>
                    <th class="text-right">Listings</th>
                    <th class="text-right">Active</th>
                    <th class="text-right">Unique Sellers</th>
                  </tr>
                </thead>
                <tbody>
                  @for (g of data.geographic; track g.location) {
                    <tr>
                      <td><strong>{{ g.location }}</strong></td>
                      <td class="text-right">{{ g.listingCount | number }}</td>
                      <td class="text-right">{{ g.active | number }}</td>
                      <td class="text-right">{{ g.uniqueSellers | number }}</td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          </div>
        } @else {
          <!-- Loading -->
          <div class="metric-grid">
            @for (i of [1,2,3,4,5,6,7,8]; track i) {
              <div class="metric-card">
                <div class="skeleton" style="width: 80px; height: 32px;"></div>
                <div class="skeleton" style="width: 100px; height: 14px; margin-top: 8px;"></div>
              </div>
            }
          </div>
          <div class="card">
            <div class="card__body">
              <div class="skeleton" style="width: 100%; height: 200px;"></div>
            </div>
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    .scraper-dash {
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

    .metric-grid {
      display: grid;
      grid-template-columns: repeat(4, 1fr);
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
        top: 0; left: 0; right: 0;
        height: 3px;
        background: var(--color-border-strong);
        border-radius: var(--radius-lg) var(--radius-lg) 0 0;
      }

      &--primary::before { background: var(--color-info); }
      &--success::before { background: var(--color-success); }
      &--warning::before { background: var(--color-warning); }
      &--accent::before { background: var(--color-accent); }

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

    .tables-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--space-6);
      margin-bottom: var(--space-6);

      @media (max-width: 768px) {
        grid-template-columns: 1fr;
      }
    }

    .table-scroll {
      overflow-x: auto;
      padding: 0;
    }

    .text-right {
      text-align: right;
    }

    .mono {
      font-family: var(--font-mono);
      font-size: var(--text-sm);
    }

    .truncate {
      max-width: 200px;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }

    /* Freshness */
    .freshness-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
      gap: var(--space-6);
    }

    .freshness-item {
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
        border-radius: var(--radius-full);
        transition: width var(--duration-slow) var(--ease-out);

        &--good { background: var(--color-success); }
        &--ok { background: var(--color-info); }
        &--stale { background: var(--color-warning); }
        &--danger { background: var(--color-danger); }
      }

      &__value {
        font-family: var(--font-display);
        font-size: var(--text-sm);
        font-weight: 600;
        color: var(--color-ink);
      }
    }

    .freshness-avg {
      margin-top: var(--space-4);
      font-size: var(--text-sm);
      color: var(--color-ink-muted);
    }
  `]
})
export class ScraperAnalyticsComponent implements OnInit {
  data: ScraperAnalyticsDashboard | null = null;

  constructor(
    private scraperAnalyticsService: ScraperAnalyticsService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.scraperAnalyticsService.getDashboard().subscribe(d => {
      this.data = d;
      this.cdr.markForCheck();
    });
  }
}
