import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { KpiService } from './kpi.service';
import { ApiService } from './api.service';
import { UserKpiDashboard, KpiSummary } from '../models';

describe('KpiService', () => {
  let service: KpiService;
  let apiSpy: jest.Mocked<ApiService>;

  const mockSummary: KpiSummary = {
    totalListings: 10,
    activeListings: 8,
    totalViews: 500,
    totalLikes: 50,
    totalFollowers: 20,
    totalMessages: 30,
    averageRating: 4.2,
    responseRate: 85,
    averageResponseTime: '2h',
    totalRevenue: 1000,
    viewsTrend: 5.5,
    likesTrend: -2.1,
    messagesTrend: 10.0
  };

  const mockDashboard: UserKpiDashboard = {
    summary: mockSummary,
    viewsOverTime: [{ date: '2026-01-01', value: 10 }],
    likesOverTime: [{ date: '2026-01-01', value: 5 }],
    messagesOverTime: [{ date: '2026-01-01', value: 2 }],
    revenueOverTime: [{ date: '2026-01-01', value: 100 }],
    categoryPerformance: [{ categoryName: 'Autos', listingCount: 3, totalViews: 200, totalLikes: 20, averageRating: 4.5 }]
  };

  beforeEach(() => {
    const api = { get: jest.fn(), post: jest.fn(), put: jest.fn(), delete: jest.fn() };
    TestBed.configureTestingModule({
      providers: [KpiService, { provide: ApiService, useValue: api }]
    });
    service = TestBed.inject(KpiService);
    apiSpy = TestBed.inject(ApiService) as jest.Mocked<ApiService>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getDashboard should call api.get with default days=30', (done) => {
    apiSpy.get.mockReturnValue(of(mockDashboard));
    service.getDashboard().subscribe(res => {
      expect(res).toEqual(mockDashboard);
      expect(apiSpy.get).toHaveBeenCalledWith('kpi/dashboard', { days: 30 });
      done();
    });
  });

  it('getDashboard should call api.get with custom days', (done) => {
    apiSpy.get.mockReturnValue(of(mockDashboard));
    service.getDashboard(7).subscribe(() => {
      expect(apiSpy.get).toHaveBeenCalledWith('kpi/dashboard', { days: 7 });
      done();
    });
  });

  it('getSummary should call api.get with kpi/summary', (done) => {
    apiSpy.get.mockReturnValue(of(mockSummary));
    service.getSummary().subscribe(res => {
      expect(res).toEqual(mockSummary);
      expect(apiSpy.get).toHaveBeenCalledWith('kpi/summary');
      done();
    });
  });
});
