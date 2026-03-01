import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ReviewService } from './review.service';
import { ApiService } from './api.service';
import { Review } from '../models';

describe('ReviewService', () => {
  let service: ReviewService;
  let apiSpy: jest.Mocked<ApiService>;

  const mockReview: Review = {
    id: 'rv1',
    listingId: 'l1',
    userId: 'u1',
    userName: 'Tester',
    rating: 4,
    comment: 'Great listing',
    status: 'Approved',
    createdAt: '2026-01-01'
  };

  beforeEach(() => {
    const api = { get: jest.fn(), post: jest.fn(), put: jest.fn(), delete: jest.fn() };
    TestBed.configureTestingModule({
      providers: [ReviewService, { provide: ApiService, useValue: api }]
    });
    service = TestBed.inject(ReviewService);
    apiSpy = TestBed.inject(ApiService) as jest.Mocked<ApiService>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getByListing should call api.get with listing id', (done) => {
    apiSpy.get.mockReturnValue(of([mockReview]));
    service.getByListing('l1').subscribe(res => {
      expect(res).toEqual([mockReview]);
      expect(apiSpy.get).toHaveBeenCalledWith('reviews/listing/l1');
      done();
    });
  });

  it('getAverage should call api.get for average', (done) => {
    apiSpy.get.mockReturnValue(of(4.5));
    service.getAverage('l1').subscribe(res => {
      expect(res).toBe(4.5);
      expect(apiSpy.get).toHaveBeenCalledWith('reviews/listing/l1/average');
      done();
    });
  });

  it('create should call api.post', (done) => {
    apiSpy.post.mockReturnValue(of(mockReview));
    const dto = { listingId: 'l1', rating: 4, comment: 'Great' };
    service.create(dto).subscribe(res => {
      expect(apiSpy.post).toHaveBeenCalledWith('reviews', dto);
      done();
    });
  });

  it('updateStatus should call api.put with stringified status', (done) => {
    apiSpy.put.mockReturnValue(of(mockReview));
    service.updateStatus('rv1', 'Approved').subscribe(() => {
      expect(apiSpy.put).toHaveBeenCalledWith('reviews/rv1/status', JSON.stringify('Approved'));
      done();
    });
  });

  it('delete should call api.delete with id', (done) => {
    apiSpy.delete.mockReturnValue(of(undefined));
    service.delete('rv1').subscribe(() => {
      expect(apiSpy.delete).toHaveBeenCalledWith('reviews/rv1');
      done();
    });
  });
});
