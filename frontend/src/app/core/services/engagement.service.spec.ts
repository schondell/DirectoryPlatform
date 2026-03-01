import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { EngagementService } from './engagement.service';
import { ApiService } from './api.service';
import { ListingEngagement, PageViewStats, VisitorStats, LikedListing, FollowedListing } from '../models';

describe('EngagementService', () => {
  let service: EngagementService;
  let apiSpy: jest.Mocked<ApiService>;

  const mockEngagement: ListingEngagement = {
    likeCount: 10,
    followerCount: 5,
    hasUserLiked: false,
    isUserFollowing: false
  };

  const mockPageViewStats: PageViewStats = {
    totalViews: 100,
    dailyViews: [{ date: '2026-01-01', viewCount: 10 }]
  };

  const mockVisitorStats: VisitorStats = {
    totalVisitors: 50,
    uniqueVisitors: 30,
    recentVisitors: [{ userId: 'u1', userName: 'Alice', visitedAt: '2026-01-01' }]
  };

  beforeEach(() => {
    const api = { get: jest.fn(), post: jest.fn(), put: jest.fn(), delete: jest.fn() };
    TestBed.configureTestingModule({
      providers: [EngagementService, { provide: ApiService, useValue: api }]
    });
    service = TestBed.inject(EngagementService);
    apiSpy = TestBed.inject(ApiService) as jest.Mocked<ApiService>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getEngagement should call api.get with listing id', (done) => {
    apiSpy.get.mockReturnValue(of(mockEngagement));
    service.getEngagement('l1').subscribe(res => {
      expect(res).toEqual(mockEngagement);
      expect(apiSpy.get).toHaveBeenCalledWith('engagement/l1');
      done();
    });
  });

  it('toggleLike should call api.post', (done) => {
    apiSpy.post.mockReturnValue(of({ isLiked: true }));
    service.toggleLike('l1').subscribe(res => {
      expect(res.isLiked).toBe(true);
      expect(apiSpy.post).toHaveBeenCalledWith('engagement/l1/like', {});
      done();
    });
  });

  it('toggleFollow should call api.post with notifyOnUpdate', (done) => {
    apiSpy.post.mockReturnValue(of({ isFollowing: true }));
    service.toggleFollow('l1', false).subscribe(res => {
      expect(res.isFollowing).toBe(true);
      expect(apiSpy.post).toHaveBeenCalledWith('engagement/l1/follow?notifyOnUpdate=false', {});
      done();
    });
  });

  it('toggleFollow should default notifyOnUpdate to true', (done) => {
    apiSpy.post.mockReturnValue(of({ isFollowing: true }));
    service.toggleFollow('l1').subscribe(() => {
      expect(apiSpy.post).toHaveBeenCalledWith('engagement/l1/follow?notifyOnUpdate=true', {});
      done();
    });
  });

  it('trackView should call api.post', (done) => {
    apiSpy.post.mockReturnValue(of(undefined));
    service.trackView('l1').subscribe(() => {
      expect(apiSpy.post).toHaveBeenCalledWith('engagement/l1/view', {});
      done();
    });
  });

  it('getPageViewStats should call api.get with days param', (done) => {
    apiSpy.get.mockReturnValue(of(mockPageViewStats));
    service.getPageViewStats('l1', 7).subscribe(res => {
      expect(res).toEqual(mockPageViewStats);
      expect(apiSpy.get).toHaveBeenCalledWith('engagement/l1/views', { days: 7 });
      done();
    });
  });

  it('getPageViewStats should default days to 30', (done) => {
    apiSpy.get.mockReturnValue(of(mockPageViewStats));
    service.getPageViewStats('l1').subscribe(() => {
      expect(apiSpy.get).toHaveBeenCalledWith('engagement/l1/views', { days: 30 });
      done();
    });
  });

  it('getVisitorStats should call api.get with days param', (done) => {
    apiSpy.get.mockReturnValue(of(mockVisitorStats));
    service.getVisitorStats('l1', 14).subscribe(res => {
      expect(res).toEqual(mockVisitorStats);
      expect(apiSpy.get).toHaveBeenCalledWith('engagement/l1/visitors', { days: 14 });
      done();
    });
  });

  it('getLikedListings should call api.get', (done) => {
    const mockLiked: LikedListing[] = [{ listingId: 'l1', title: 'Test', likedAt: '2026-01-01' }];
    apiSpy.get.mockReturnValue(of(mockLiked));
    service.getLikedListings().subscribe(res => {
      expect(res).toEqual(mockLiked);
      expect(apiSpy.get).toHaveBeenCalledWith('engagement/liked');
      done();
    });
  });

  it('getFollowedListings should call api.get', (done) => {
    const mockFollowed: FollowedListing[] = [{ listingId: 'l1', title: 'Test', notifyOnUpdate: true, followedAt: '2026-01-01' }];
    apiSpy.get.mockReturnValue(of(mockFollowed));
    service.getFollowedListings().subscribe(res => {
      expect(res).toEqual(mockFollowed);
      expect(apiSpy.get).toHaveBeenCalledWith('engagement/followed');
      done();
    });
  });
});
