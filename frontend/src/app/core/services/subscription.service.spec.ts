import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { SubscriptionService } from './subscription.service';
import { ApiService } from './api.service';
import { Subscription, SubscriptionTier } from '../models';

describe('SubscriptionService', () => {
  let service: SubscriptionService;
  let apiSpy: jest.Mocked<ApiService>;

  const mockSubscription: Subscription = {
    id: 's1',
    userId: 'u1',
    subscriptionTierId: 'st1',
    tierName: 'Standard',
    startDate: '2026-01-01',
    endDate: '2026-02-01',
    isActive: true,
    autoRenew: true,
    createdAt: '2026-01-01'
  };

  const mockTier: SubscriptionTier = {
    id: 'st1',
    name: 'Standard',
    description: 'Basic plan',
    monthlyPrice: 9.90,
    isActive: true,
    features: [{ id: 'f1', name: 'Listings', value: '15', isEnabled: true }]
  };

  beforeEach(() => {
    const api = { get: jest.fn(), post: jest.fn(), put: jest.fn(), delete: jest.fn() };
    TestBed.configureTestingModule({
      providers: [SubscriptionService, { provide: ApiService, useValue: api }]
    });
    service = TestBed.inject(SubscriptionService);
    apiSpy = TestBed.inject(ApiService) as jest.Mocked<ApiService>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getActive should call api.get with subscriptions/my', (done) => {
    apiSpy.get.mockReturnValue(of(mockSubscription));
    service.getActive().subscribe(res => {
      expect(res).toEqual(mockSubscription);
      expect(apiSpy.get).toHaveBeenCalledWith('subscriptions/my');
      done();
    });
  });

  it('getTiers should call api.get with subscriptions/tiers', (done) => {
    apiSpy.get.mockReturnValue(of([mockTier]));
    service.getTiers().subscribe(res => {
      expect(res).toEqual([mockTier]);
      expect(apiSpy.get).toHaveBeenCalledWith('subscriptions/tiers');
      done();
    });
  });

  it('create should call api.post', (done) => {
    apiSpy.post.mockReturnValue(of(mockSubscription));
    service.create({ tierId: 'st1' }).subscribe(res => {
      expect(apiSpy.post).toHaveBeenCalledWith('subscriptions', { tierId: 'st1' });
      done();
    });
  });

  it('cancel should call api.put with id', (done) => {
    apiSpy.put.mockReturnValue(of(undefined));
    service.cancel('s1').subscribe(() => {
      expect(apiSpy.put).toHaveBeenCalledWith('subscriptions/s1/cancel', {});
      done();
    });
  });
});
