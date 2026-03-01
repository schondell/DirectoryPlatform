import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ListingService } from './listing.service';
import { ApiService } from './api.service';
import { Listing, PagedResult, ListingFilterRequest } from '../models';

describe('ListingService', () => {
  let service: ListingService;
  let apiSpy: jest.Mocked<ApiService>;

  const mockListing: Listing = {
    id: 'l1',
    title: 'Test Listing',
    status: 'Active',
    categoryId: 'c1',
    weight: 1,
    isFeatured: false,
    isPremium: false,
    viewCount: 10,
    userId: 'u1',
    createdAt: '2026-01-01',
    attributes: [],
    media: []
  };

  const mockPagedResult: PagedResult<Listing> = {
    items: [mockListing],
    totalCount: 1,
    pageNumber: 1,
    pageSize: 20,
    totalPages: 1,
    hasNextPage: false,
    hasPreviousPage: false
  };

  beforeEach(() => {
    const api = {
      get: jest.fn(),
      post: jest.fn(),
      put: jest.fn(),
      delete: jest.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        ListingService,
        { provide: ApiService, useValue: api }
      ]
    });
    service = TestBed.inject(ListingService);
    apiSpy = TestBed.inject(ApiService) as jest.Mocked<ApiService>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getFiltered', () => {
    it('should call api.get with default params', (done) => {
      apiSpy.get.mockReturnValue(of(mockPagedResult));
      service.getFiltered({}).subscribe(res => {
        expect(res).toEqual(mockPagedResult);
        expect(apiSpy.get).toHaveBeenCalledWith('listings', { pageNumber: 1, pageSize: 20 });
        done();
      });
    });

    it('should include search, category, region, sort params', (done) => {
      apiSpy.get.mockReturnValue(of(mockPagedResult));
      const filter: ListingFilterRequest = {
        pageNumber: 2,
        pageSize: 10,
        searchTerm: 'car',
        categoryId: 'c1',
        regionId: 'r1',
        sortBy: 'title',
        ascending: true
      };
      service.getFiltered(filter).subscribe(() => {
        expect(apiSpy.get).toHaveBeenCalledWith('listings', {
          pageNumber: 2,
          pageSize: 10,
          search: 'car',
          categoryId: 'c1',
          regionId: 'r1',
          sortBy: 'title',
          ascending: true
        });
        done();
      });
    });

    it('should include attribute filters', (done) => {
      apiSpy.get.mockReturnValue(of(mockPagedResult));
      const filter: ListingFilterRequest = {
        attributes: { brand: 'BMW', color: 'red' }
      };
      service.getFiltered(filter).subscribe(() => {
        const callArgs = apiSpy.get.mock.calls[0][1] as Record<string, any>;
        expect(callArgs['attr[brand]']).toBe('BMW');
        expect(callArgs['attr[color]']).toBe('red');
        done();
      });
    });
  });

  describe('getById', () => {
    it('should call api.get with listing id', (done) => {
      apiSpy.get.mockReturnValue(of(mockListing));
      service.getById('l1').subscribe(res => {
        expect(res).toEqual(mockListing);
        expect(apiSpy.get).toHaveBeenCalledWith('listings/l1');
        done();
      });
    });
  });

  describe('getFeatured', () => {
    it('should call api.get with default count', (done) => {
      apiSpy.get.mockReturnValue(of([mockListing]));
      service.getFeatured().subscribe(res => {
        expect(res).toEqual([mockListing]);
        expect(apiSpy.get).toHaveBeenCalledWith('listings/featured', { count: 10 });
        done();
      });
    });

    it('should call api.get with custom count', (done) => {
      apiSpy.get.mockReturnValue(of([mockListing]));
      service.getFeatured(5).subscribe(() => {
        expect(apiSpy.get).toHaveBeenCalledWith('listings/featured', { count: 5 });
        done();
      });
    });
  });

  describe('getRecent', () => {
    it('should call api.get with count param', (done) => {
      apiSpy.get.mockReturnValue(of([mockListing]));
      service.getRecent(3).subscribe(() => {
        expect(apiSpy.get).toHaveBeenCalledWith('listings/recent', { count: 3 });
        done();
      });
    });
  });

  describe('getMyListings', () => {
    it('should call api.get for my listings', (done) => {
      apiSpy.get.mockReturnValue(of([mockListing]));
      service.getMyListings().subscribe(res => {
        expect(res).toEqual([mockListing]);
        expect(apiSpy.get).toHaveBeenCalledWith('listings/my');
        done();
      });
    });
  });

  describe('create', () => {
    it('should call api.post', (done) => {
      apiSpy.post.mockReturnValue(of(mockListing));
      service.create({ title: 'New' }).subscribe(res => {
        expect(res).toEqual(mockListing);
        expect(apiSpy.post).toHaveBeenCalledWith('listings', { title: 'New' });
        done();
      });
    });
  });

  describe('update', () => {
    it('should call api.put with id', (done) => {
      apiSpy.put.mockReturnValue(of(mockListing));
      service.update('l1', { title: 'Updated' }).subscribe(res => {
        expect(res).toEqual(mockListing);
        expect(apiSpy.put).toHaveBeenCalledWith('listings/l1', { title: 'Updated' });
        done();
      });
    });
  });

  describe('delete', () => {
    it('should call api.delete with id', (done) => {
      apiSpy.delete.mockReturnValue(of(undefined));
      service.delete('l1').subscribe(() => {
        expect(apiSpy.delete).toHaveBeenCalledWith('listings/l1');
        done();
      });
    });
  });
});
