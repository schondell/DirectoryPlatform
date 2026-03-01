import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { ListingGridComponent } from './listing-grid.component';
import { ListingStateService } from '../../../core/state/listing-state.service';
import { CategoryService } from '../../../core/services/category.service';
import { RegionService } from '../../../core/services/region.service';
import { TranslateModule } from '@ngx-translate/core';
import { signal, computed } from '@angular/core';
import { CategoryWithChildren, RegionWithChildren } from '../../../core/models';

describe('ListingGridComponent', () => {
  let component: ListingGridComponent;
  let fixture: ComponentFixture<ListingGridComponent>;
  let categoryServiceSpy: jest.Mocked<Partial<CategoryService>>;
  let regionServiceSpy: jest.Mocked<Partial<RegionService>>;
  let mockState: any;

  const mockCategories: CategoryWithChildren[] = [
    { id: 'c1', name: 'Automobiles', slug: 'automobiles', displayOrder: 1, children: [] }
  ];

  const mockRegions: RegionWithChildren[] = [
    { id: 'r1', name: 'Zurich', slug: 'zurich', displayOrder: 1, children: [] }
  ];

  beforeEach(async () => {
    categoryServiceSpy = {
      getTree: jest.fn().mockReturnValue(of(mockCategories))
    };
    regionServiceSpy = {
      getTree: jest.fn().mockReturnValue(of(mockRegions))
    };
    mockState = {
      listings: signal([]),
      loading: signal(false),
      error: signal(null),
      totalCount: signal(0),
      pageNumber: signal(1),
      pageSize: signal(20),
      totalPages: computed(() => 0),
      hasNextPage: computed(() => false),
      hasPreviousPage: computed(() => false),
      loadListings: jest.fn(),
      setFilters: jest.fn(),
      setAttributeFilter: jest.fn(),
      clearAttributeFilters: jest.fn(),
      setPage: jest.fn()
    };

    await TestBed.configureTestingModule({
      imports: [ListingGridComponent, TranslateModule.forRoot()],
      providers: [
        { provide: CategoryService, useValue: categoryServiceSpy },
        { provide: RegionService, useValue: regionServiceSpy },
        { provide: ListingStateService, useValue: mockState },
        { provide: ActivatedRoute, useValue: { queryParams: of({}) } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ListingGridComponent);
    component = fixture.componentInstance;
    // Override the injected state
    component.state = mockState;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load categories on init', () => {
    expect(categoryServiceSpy.getTree).toHaveBeenCalled();
    expect(component.categories).toEqual(mockCategories);
  });

  it('should load regions on init', () => {
    expect(regionServiceSpy.getTree).toHaveBeenCalled();
    expect(component.regions).toEqual(mockRegions);
  });

  it('should call loadListings when no query params', () => {
    expect(mockState.loadListings).toHaveBeenCalled();
  });

  it('should apply categoryId from query params', async () => {
    TestBed.resetTestingModule();
    categoryServiceSpy.getTree = jest.fn().mockReturnValue(of(mockCategories));
    regionServiceSpy.getTree = jest.fn().mockReturnValue(of(mockRegions));

    await TestBed.configureTestingModule({
      imports: [ListingGridComponent, TranslateModule.forRoot()],
      providers: [
        { provide: CategoryService, useValue: categoryServiceSpy },
        { provide: RegionService, useValue: regionServiceSpy },
        { provide: ListingStateService, useValue: mockState },
        { provide: ActivatedRoute, useValue: { queryParams: of({ categoryId: 'c1' }) } }
      ]
    }).compileComponents();

    const fix = TestBed.createComponent(ListingGridComponent);
    const comp = fix.componentInstance;
    comp.state = mockState;
    fix.detectChanges();

    expect(comp.selectedCategoryId).toBe('c1');
    expect(mockState.setFilters).toHaveBeenCalledWith({ categoryId: 'c1' });
  });

  it('onCategoryChange should clear attribute filters and set category filter', () => {
    component.onCategoryChange('c2');
    expect(mockState.clearAttributeFilters).toHaveBeenCalled();
    expect(mockState.setFilters).toHaveBeenCalledWith({ categoryId: 'c2' });
  });

  it('onCategoryChange with empty string should pass undefined', () => {
    component.onCategoryChange('');
    expect(mockState.setFilters).toHaveBeenCalledWith({ categoryId: undefined });
  });

  it('onRegionChange should set region filter', () => {
    component.onRegionChange('r1');
    expect(mockState.setFilters).toHaveBeenCalledWith({ regionId: 'r1' });
  });

  it('onSearch should set searchTerm filter', () => {
    component.searchTerm = 'test search';
    component.onSearch();
    expect(mockState.setFilters).toHaveBeenCalledWith({ searchTerm: 'test search' });
  });

  it('onSearch with empty string should pass undefined', () => {
    component.searchTerm = '';
    component.onSearch();
    expect(mockState.setFilters).toHaveBeenCalledWith({ searchTerm: undefined });
  });

  it('onSortChange should set sortBy filter', () => {
    component.onSortChange('title');
    expect(mockState.setFilters).toHaveBeenCalledWith({ sortBy: 'title' });
  });
});
