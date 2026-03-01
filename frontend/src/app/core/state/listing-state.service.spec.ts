import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { ListingStateService } from './listing-state.service';
import { ListingService } from '../services/listing.service';
import { Listing, PagedResult } from '../models';

describe('ListingStateService', () => {
  let service: ListingStateService;
  let listingServiceSpy: jest.Mocked<ListingService>;

  const mockListing: Listing = {
    id: 'l1', title: 'Test', status: 'Active', categoryId: 'c1',
    weight: 1, isFeatured: false, isPremium: false, viewCount: 10,
    userId: 'u1', createdAt: '2026-01-01', attributes: [], media: []
  };

  const mockPagedResult: PagedResult<Listing> = {
    items: [mockListing],
    totalCount: 50,
    pageNumber: 1,
    pageSize: 20,
    totalPages: 3,
    hasNextPage: true,
    hasPreviousPage: false
  };

  beforeEach(() => {
    const listingSvc = {
      getFiltered: jest.fn(),
      getById: jest.fn(),
      getFeatured: jest.fn(),
      getRecent: jest.fn(),
      getMyListings: jest.fn(),
      create: jest.fn(),
      update: jest.fn(),
      delete: jest.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        ListingStateService,
        { provide: ListingService, useValue: listingSvc }
      ]
    });
    service = TestBed.inject(ListingStateService);
    listingServiceSpy = TestBed.inject(ListingService) as jest.Mocked<ListingService>;
  });

  it('should be created with default state', () => {
    expect(service).toBeTruthy();
    expect(service.listings()).toEqual([]);
    expect(service.loading()).toBe(false);
    expect(service.error()).toBeNull();
    expect(service.totalCount()).toBe(0);
    expect(service.pageNumber()).toBe(1);
    expect(service.pageSize()).toBe(20);
  });

  it('should have correct computed properties', () => {
    expect(service.totalPages()).toBe(0);
    expect(service.hasNextPage()).toBe(false);
    expect(service.hasPreviousPage()).toBe(false);
  });

  describe('loadListings', () => {
    it('should set loading, fetch listings, and update state on success', () => {
      listingServiceSpy.getFiltered.mockReturnValue(of(mockPagedResult));
      service.loadListings();
      expect(service.loading()).toBe(false);
      expect(service.listings()).toEqual([mockListing]);
      expect(service.totalCount()).toBe(50);
      expect(service.error()).toBeNull();
    });

    it('should set error on failure', () => {
      listingServiceSpy.getFiltered.mockReturnValue(throwError(() => new Error('Network error')));
      service.loadListings();
      expect(service.loading()).toBe(false);
      expect(service.error()).toBe('Network error');
      expect(service.listings()).toEqual([]);
    });

    it('should include attribute filters in the request', () => {
      listingServiceSpy.getFiltered.mockReturnValue(of(mockPagedResult));
      service.setAttributeFilter('brand', 'BMW');
      // setAttributeFilter calls loadListings internally
      const callArgs = listingServiceSpy.getFiltered.mock.calls[0][0];
      expect(callArgs.attributes).toEqual({ brand: 'BMW' });
    });
  });

  describe('setFilters', () => {
    it('should update filters and reset page to 1', () => {
      listingServiceSpy.getFiltered.mockReturnValue(of(mockPagedResult));
      service.setPage(3);
      service.setFilters({ categoryId: 'c1', searchTerm: 'test' });
      expect(service.pageNumber()).toBe(1);
      expect(listingServiceSpy.getFiltered).toHaveBeenCalled();
    });

    it('should merge new filters with existing ones', () => {
      listingServiceSpy.getFiltered.mockReturnValue(of(mockPagedResult));
      service.setFilters({ categoryId: 'c1' });
      service.setFilters({ searchTerm: 'test' });
      const lastCall = listingServiceSpy.getFiltered.mock.calls[listingServiceSpy.getFiltered.mock.calls.length - 1][0];
      expect(lastCall.categoryId).toBe('c1');
      expect(lastCall.searchTerm).toBe('test');
    });
  });

  describe('setAttributeFilter', () => {
    it('should add attribute filter and reload', () => {
      listingServiceSpy.getFiltered.mockReturnValue(of(mockPagedResult));
      service.setAttributeFilter('color', 'red');
      expect(service.attributeFilters()).toEqual({ color: 'red' });
      expect(service.pageNumber()).toBe(1);
    });

    it('should remove attribute filter when value is empty', () => {
      listingServiceSpy.getFiltered.mockReturnValue(of(mockPagedResult));
      service.setAttributeFilter('color', 'red');
      service.setAttributeFilter('color', '');
      expect(service.attributeFilters()).toEqual({});
    });
  });

  describe('clearAttributeFilters', () => {
    it('should clear all attribute filters and reload', () => {
      listingServiceSpy.getFiltered.mockReturnValue(of(mockPagedResult));
      service.setAttributeFilter('color', 'red');
      service.setAttributeFilter('brand', 'BMW');
      service.clearAttributeFilters();
      expect(service.attributeFilters()).toEqual({});
    });
  });

  describe('setPage', () => {
    it('should update page number and reload', () => {
      listingServiceSpy.getFiltered.mockReturnValue(of(mockPagedResult));
      service.setPage(2);
      expect(service.pageNumber()).toBe(2);
      expect(listingServiceSpy.getFiltered).toHaveBeenCalled();
    });
  });

  describe('computed totalPages', () => {
    it('should calculate total pages correctly', () => {
      listingServiceSpy.getFiltered.mockReturnValue(of(mockPagedResult));
      service.loadListings();
      // totalCount = 50, pageSize = 20 => ceil(50/20) = 3
      expect(service.totalPages()).toBe(3);
    });
  });

  describe('computed hasNextPage', () => {
    it('should be true when more pages exist', () => {
      listingServiceSpy.getFiltered.mockReturnValue(of(mockPagedResult));
      service.loadListings();
      // pageNumber=1, totalPages=3
      expect(service.hasNextPage()).toBe(true);
    });
  });

  describe('computed hasPreviousPage', () => {
    it('should be false on first page', () => {
      expect(service.hasPreviousPage()).toBe(false);
    });

    it('should be true on page > 1', () => {
      listingServiceSpy.getFiltered.mockReturnValue(of(mockPagedResult));
      service.setPage(2);
      expect(service.hasPreviousPage()).toBe(true);
    });
  });
});
