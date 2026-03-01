import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { BoostService } from './boost.service';
import { ApiService } from './api.service';
import { Boost, BoostPricing, CreateBoost } from '../models';

describe('BoostService', () => {
  let service: BoostService;
  let apiSpy: jest.Mocked<ApiService>;

  const mockBoost: Boost = {
    id: 'b1',
    listingId: 'l1',
    boostType: 'Standard',
    startsAt: '2026-01-01',
    expiresAt: '2026-01-08',
    multiplier: 2,
    amountPaid: 14.00,
    currency: 'CHF',
    isActive: true
  };

  const mockPricing: BoostPricing = {
    boostType: 'Standard',
    dailyRate: 2.00,
    multiplier: 2,
    description: 'Standard boost'
  };

  beforeEach(() => {
    const api = { get: jest.fn(), post: jest.fn(), put: jest.fn(), delete: jest.fn() };
    TestBed.configureTestingModule({
      providers: [BoostService, { provide: ApiService, useValue: api }]
    });
    service = TestBed.inject(BoostService);
    apiSpy = TestBed.inject(ApiService) as jest.Mocked<ApiService>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getPricing should call api.get with boost/pricing', (done) => {
    apiSpy.get.mockReturnValue(of([mockPricing]));
    service.getPricing().subscribe(res => {
      expect(res).toEqual([mockPricing]);
      expect(apiSpy.get).toHaveBeenCalledWith('boost/pricing');
      done();
    });
  });

  it('createBoost should call api.post with dto', (done) => {
    apiSpy.post.mockReturnValue(of(mockBoost));
    const dto: CreateBoost = { listingId: 'l1', boostType: 'Standard', durationDays: 7 };
    service.createBoost(dto).subscribe(res => {
      expect(res).toEqual(mockBoost);
      expect(apiSpy.post).toHaveBeenCalledWith('boost', dto);
      done();
    });
  });

  it('getActiveBoosts should call api.get with listing id', (done) => {
    apiSpy.get.mockReturnValue(of([mockBoost]));
    service.getActiveBoosts('l1').subscribe(res => {
      expect(res).toEqual([mockBoost]);
      expect(apiSpy.get).toHaveBeenCalledWith('boost/listing/l1');
      done();
    });
  });

  it('getMyBoosts should call api.get with boost/my', (done) => {
    apiSpy.get.mockReturnValue(of([mockBoost]));
    service.getMyBoosts().subscribe(res => {
      expect(res).toEqual([mockBoost]);
      expect(apiSpy.get).toHaveBeenCalledWith('boost/my');
      done();
    });
  });
});
