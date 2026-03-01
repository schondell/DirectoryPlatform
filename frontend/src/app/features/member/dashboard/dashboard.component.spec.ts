import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { MemberDashboardComponent } from './dashboard.component';
import { KpiService } from '../../../core/services/kpi.service';
import { TranslateModule } from '@ngx-translate/core';
import { UserKpiDashboard, KpiTimeSeries } from '../../../core/models';

describe('MemberDashboardComponent', () => {
  let component: MemberDashboardComponent;
  let fixture: ComponentFixture<MemberDashboardComponent>;
  let kpiServiceSpy: jest.Mocked<Partial<KpiService>>;

  const mockDashboard: UserKpiDashboard = {
    summary: {
      totalListings: 10, activeListings: 8, totalViews: 500, totalLikes: 50,
      totalFollowers: 20, totalMessages: 30, averageRating: 4.2, responseRate: 85,
      averageResponseTime: '2h', totalRevenue: 1000, viewsTrend: 5.5,
      likesTrend: -2.1, messagesTrend: 10.0
    },
    viewsOverTime: [
      { date: '2026-01-01', value: 10 },
      { date: '2026-01-02', value: 20 },
      { date: '2026-01-03', value: 5 }
    ],
    likesOverTime: [{ date: '2026-01-01', value: 5 }],
    messagesOverTime: [{ date: '2026-01-01', value: 2 }],
    revenueOverTime: [{ date: '2026-01-01', value: 100 }],
    categoryPerformance: [
      { categoryName: 'Autos', listingCount: 3, totalViews: 200, totalLikes: 20, averageRating: 4.5 }
    ]
  };

  beforeEach(async () => {
    kpiServiceSpy = {
      getDashboard: jest.fn().mockReturnValue(of(mockDashboard))
    };

    await TestBed.configureTestingModule({
      imports: [MemberDashboardComponent, TranslateModule.forRoot()],
      providers: [
        provideRouter([]),
        { provide: KpiService, useValue: kpiServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MemberDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load dashboard data on init', () => {
    expect(kpiServiceSpy.getDashboard).toHaveBeenCalledWith(30);
    expect(component.dashboard).toEqual(mockDashboard);
  });

  it('should render KPI cards when dashboard data is loaded', () => {
    const el = fixture.nativeElement as HTMLElement;
    const kpiCards = el.querySelectorAll('.kpi-card');
    expect(kpiCards.length).toBeGreaterThan(0);
  });

  it('should render sparkline bars', () => {
    const el = fixture.nativeElement as HTMLElement;
    const bars = el.querySelectorAll('.sparkline__bar');
    expect(bars.length).toBe(3); // 3 viewsOverTime data points
  });

  it('should render category performance table', () => {
    const el = fixture.nativeElement as HTMLElement;
    const tableRows = el.querySelectorAll('.table tbody tr');
    expect(tableRows.length).toBe(1);
    expect(tableRows[0].textContent).toContain('Autos');
  });

  describe('getBarHeight', () => {
    it('should return percentage of max value', () => {
      const series: KpiTimeSeries[] = [
        { date: '2026-01-01', value: 10 },
        { date: '2026-01-02', value: 20 }
      ];
      expect(component.getBarHeight(20, series)).toBe(100);
      expect(component.getBarHeight(10, series)).toBe(50);
    });

    it('should return minimum 2% for zero values', () => {
      const series: KpiTimeSeries[] = [
        { date: '2026-01-01', value: 0 },
        { date: '2026-01-02', value: 10 }
      ];
      expect(component.getBarHeight(0, series)).toBe(2);
    });

    it('should handle all-zero series', () => {
      const series: KpiTimeSeries[] = [
        { date: '2026-01-01', value: 0 }
      ];
      // Math.max(...[0], 1) = 1, so (0/1)*100 = 0, max(0, 2) = 2
      expect(component.getBarHeight(0, series)).toBe(2);
    });
  });

  it('should show loading skeleton when dashboard is null', async () => {
    // Create a fresh component where the service returns data that hasn't loaded yet
    kpiServiceSpy.getDashboard = jest.fn().mockReturnValue(of(null as any));

    TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [MemberDashboardComponent, TranslateModule.forRoot()],
      providers: [
        provideRouter([]),
        { provide: KpiService, useValue: kpiServiceSpy }
      ]
    }).compileComponents();

    const fix2 = TestBed.createComponent(MemberDashboardComponent);
    const comp2 = fix2.componentInstance;
    fix2.detectChanges();

    const el = fix2.nativeElement as HTMLElement;
    const skeletons = el.querySelectorAll('.skeleton');
    expect(skeletons.length).toBeGreaterThan(0);
  });
});
