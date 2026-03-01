import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { AdminDashboardComponent } from './dashboard.component';
import { AdminDashboardService } from '../../../core/services/admin-dashboard.service';
import { TranslateModule } from '@ngx-translate/core';
import { AdminDashboard } from '../../../core/models';

describe('AdminDashboardComponent', () => {
  let component: AdminDashboardComponent;
  let fixture: ComponentFixture<AdminDashboardComponent>;
  let adminDashboardServiceSpy: jest.Mocked<Partial<AdminDashboardService>>;

  const mockDashboard: AdminDashboard = {
    overview: {
      totalUsers: 100, totalListings: 500, activeListings: 400,
      pendingApprovals: 10, monthlyRevenue: 5000,
      newUsersThisMonth: 15, newListingsThisMonth: 30
    },
    recentActivity: {
      recentUsers: [{ id: 'u1', username: 'alice', email: 'alice@test.com', createdAt: '2026-01-01' }],
      recentListings: [{ id: 'l1', title: 'Test Listing', status: 'Active', createdAt: '2026-01-01' }]
    },
    systemHealth: {
      cpuUsagePercent: 25, memoryUsageMB: 512, memoryTotalMB: 2048,
      dotNetVersion: '10.0', osPlatform: 'Linux', uptime: '5d 3h'
    }
  };

  beforeEach(async () => {
    adminDashboardServiceSpy = {
      getDashboard: jest.fn().mockReturnValue(of(mockDashboard))
    };

    await TestBed.configureTestingModule({
      imports: [AdminDashboardComponent, TranslateModule.forRoot()],
      providers: [
        provideRouter([]),
        { provide: AdminDashboardService, useValue: adminDashboardServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load dashboard data on init', () => {
    expect(adminDashboardServiceSpy.getDashboard).toHaveBeenCalled();
    expect(component.data).toEqual(mockDashboard);
  });

  it('should render metric cards', () => {
    const el = fixture.nativeElement as HTMLElement;
    const metricCards = el.querySelectorAll('.metric-card');
    expect(metricCards.length).toBe(5);
  });

  it('should render recent users table', () => {
    const el = fixture.nativeElement as HTMLElement;
    const tables = el.querySelectorAll('.table');
    expect(tables.length).toBe(2); // users + listings
  });

  it('should render system health info', () => {
    const el = fixture.nativeElement as HTMLElement;
    const healthItems = el.querySelectorAll('.health-item');
    expect(healthItems.length).toBe(5); // CPU, Memory, .NET, Platform, Uptime
  });

  describe('getStatusBadge', () => {
    it('should return correct badge class for active status', () => {
      expect(component.getStatusBadge('Active')).toBe('badge--success');
      expect(component.getStatusBadge('active')).toBe('badge--success');
    });

    it('should return correct badge class for pending status', () => {
      expect(component.getStatusBadge('Pending')).toBe('badge--warning');
    });

    it('should return correct badge class for rejected status', () => {
      expect(component.getStatusBadge('Rejected')).toBe('badge--danger');
    });

    it('should return correct badge class for expired status', () => {
      expect(component.getStatusBadge('Expired')).toBe('badge--neutral');
    });

    it('should return neutral for unknown status', () => {
      expect(component.getStatusBadge('Unknown')).toBe('badge--neutral');
    });
  });

  it('should show loading skeleton when data is null', async () => {
    // Create a fresh component where the service returns null (not yet loaded)
    adminDashboardServiceSpy.getDashboard = jest.fn().mockReturnValue(of(null as any));

    TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [AdminDashboardComponent, TranslateModule.forRoot()],
      providers: [
        provideRouter([]),
        { provide: AdminDashboardService, useValue: adminDashboardServiceSpy }
      ]
    }).compileComponents();

    const fix2 = TestBed.createComponent(AdminDashboardComponent);
    fix2.detectChanges();

    const el = fix2.nativeElement as HTMLElement;
    const skeletons = el.querySelectorAll('.skeleton');
    expect(skeletons.length).toBeGreaterThan(0);
  });
});
