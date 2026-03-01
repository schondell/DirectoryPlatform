import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { AdminDashboardService } from './admin-dashboard.service';
import { ApiService } from './api.service';
import { AdminDashboard } from '../models';

describe('AdminDashboardService', () => {
  let service: AdminDashboardService;
  let apiSpy: jest.Mocked<ApiService>;

  const mockDashboard: AdminDashboard = {
    overview: {
      totalUsers: 100,
      totalListings: 500,
      activeListings: 400,
      pendingApprovals: 10,
      monthlyRevenue: 5000,
      newUsersThisMonth: 15,
      newListingsThisMonth: 30
    },
    recentActivity: {
      recentUsers: [{ id: 'u1', username: 'alice', email: 'alice@test.com', createdAt: '2026-01-01' }],
      recentListings: [{ id: 'l1', title: 'Test Listing', status: 'Active', createdAt: '2026-01-01' }]
    },
    systemHealth: {
      cpuUsagePercent: 25,
      memoryUsageMB: 512,
      memoryTotalMB: 2048,
      dotNetVersion: '10.0',
      osPlatform: 'Linux',
      uptime: '5d 3h'
    }
  };

  beforeEach(() => {
    const api = { get: jest.fn(), post: jest.fn(), put: jest.fn(), delete: jest.fn() };
    TestBed.configureTestingModule({
      providers: [AdminDashboardService, { provide: ApiService, useValue: api }]
    });
    service = TestBed.inject(AdminDashboardService);
    apiSpy = TestBed.inject(ApiService) as jest.Mocked<ApiService>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getDashboard should call api.get with admin/admindashboard', (done) => {
    apiSpy.get.mockReturnValue(of(mockDashboard));
    service.getDashboard().subscribe(res => {
      expect(res).toEqual(mockDashboard);
      expect(apiSpy.get).toHaveBeenCalledWith('admin/admindashboard');
      done();
    });
  });
});
