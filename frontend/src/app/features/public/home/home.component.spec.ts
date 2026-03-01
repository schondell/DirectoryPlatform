import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { HomeComponent } from './home.component';
import { ListingService } from '../../../core/services/listing.service';
import { CategoryService } from '../../../core/services/category.service';
import { TranslateModule } from '@ngx-translate/core';
import { CategoryWithChildren, Listing } from '../../../core/models';

describe('HomeComponent', () => {
  let component: HomeComponent;
  let fixture: ComponentFixture<HomeComponent>;
  let listingServiceSpy: jest.Mocked<Partial<ListingService>>;
  let categoryServiceSpy: jest.Mocked<Partial<CategoryService>>;
  let router: Router;

  const mockCategories: CategoryWithChildren[] = [
    { id: 'c1', name: 'Automobiles', slug: 'automobiles', displayOrder: 1, children: [
      { id: 'c2', name: 'Cars', slug: 'cars', parentId: 'c1', displayOrder: 1, children: [] }
    ] }
  ];

  const mockListings: Listing[] = [
    {
      id: 'l1', title: 'Test Car', status: 'Active', categoryId: 'c1',
      weight: 1, isFeatured: true, isPremium: false, viewCount: 100,
      userId: 'u1', createdAt: '2026-01-01', attributes: [], media: [],
      category: { id: 'c1', name: 'Automobiles', slug: 'automobiles', displayOrder: 1 }
    }
  ];

  beforeEach(async () => {
    listingServiceSpy = {
      getFeatured: jest.fn().mockReturnValue(of(mockListings)),
      getRecent: jest.fn().mockReturnValue(of(mockListings))
    };
    categoryServiceSpy = {
      getTree: jest.fn().mockReturnValue(of(mockCategories))
    };

    await TestBed.configureTestingModule({
      imports: [HomeComponent, TranslateModule.forRoot()],
      providers: [
        provideRouter([]),
        { provide: ListingService, useValue: listingServiceSpy },
        { provide: CategoryService, useValue: categoryServiceSpy }
      ]
    }).compileComponents();

    router = TestBed.inject(Router);
    jest.spyOn(router, 'navigate').mockResolvedValue(true);

    fixture = TestBed.createComponent(HomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load categories on init', () => {
    expect(categoryServiceSpy.getTree).toHaveBeenCalled();
    expect(component.categories).toEqual(mockCategories);
  });

  it('should load featured listings on init', () => {
    expect(listingServiceSpy.getFeatured).toHaveBeenCalledWith(6);
    expect(component.featuredListings).toEqual(mockListings);
  });

  it('should load recent listings on init', () => {
    expect(listingServiceSpy.getRecent).toHaveBeenCalledWith(6);
    expect(component.recentListings).toEqual(mockListings);
  });

  it('should navigate with search term on onSearch', () => {
    component.searchTerm = 'car';
    component.selectedCategoryId = 'c1';
    component.onSearch();
    expect(router.navigate).toHaveBeenCalledWith(['/listings'], {
      queryParams: { search: 'car', categoryId: 'c1' }
    });
  });

  it('should navigate with only search term when no category selected', () => {
    component.searchTerm = 'bike';
    component.selectedCategoryId = '';
    component.onSearch();
    expect(router.navigate).toHaveBeenCalledWith(['/listings'], {
      queryParams: { search: 'bike' }
    });
  });

  it('should navigate with empty params when both are empty', () => {
    component.searchTerm = '';
    component.selectedCategoryId = '';
    component.onSearch();
    expect(router.navigate).toHaveBeenCalledWith(['/listings'], {
      queryParams: {}
    });
  });

  it('getCategoryIcon should return correct icon for known slug', () => {
    expect(component.getCategoryIcon('automobiles')).toBe('\uD83D\uDE97');
    expect(component.getCategoryIcon('immobilier')).toBe('\uD83C\uDFE0');
  });

  it('getCategoryIcon should return default icon for unknown slug', () => {
    expect(component.getCategoryIcon('unknown-slug')).toBe('\uD83D\uDCC2');
  });

  it('should render category cards', () => {
    const el = fixture.nativeElement as HTMLElement;
    const catCards = el.querySelectorAll('.cat-card');
    expect(catCards.length).toBe(1);
  });
});
