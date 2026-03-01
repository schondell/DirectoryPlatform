import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { ListingDetailComponent } from './listing-detail.component';
import { ListingService } from '../../../core/services/listing.service';
import { TranslateModule } from '@ngx-translate/core';
import { Listing } from '../../../core/models';

describe('ListingDetailComponent', () => {
  let component: ListingDetailComponent;
  let fixture: ComponentFixture<ListingDetailComponent>;
  let listingServiceSpy: jest.Mocked<Partial<ListingService>>;

  const mockListing: Listing = {
    id: 'l1', title: 'Nice Car', shortDescription: 'A very nice car',
    description: '<p>Full description</p>', status: 'Active', categoryId: 'c1',
    category: { id: 'c1', name: 'Automobiles', slug: 'automobiles', displayOrder: 1 },
    regionId: 'r1',
    region: { id: 'r1', name: 'Zurich', slug: 'zurich', displayOrder: 1 },
    weight: 1, isFeatured: false, isPremium: false, viewCount: 50,
    userId: 'u1', createdAt: '2026-01-01',
    attributes: [
      { id: 'a1', attributeDefinitionId: 'ad1', attributeName: 'Brand', attributeSlug: 'brand', value: 'BMW', displayOrder: 1 }
    ],
    media: [
      { id: 'm1', url: 'http://img.jpg', mediaType: 'image', displayOrder: 1, isPrimary: true }
    ],
    detail: {
      address: '123 Main St', phone: '+41 123 456', email: 'contact@test.com',
      website: 'https://example.com', priceInfo: 'CHF 25000'
    }
  };

  beforeEach(async () => {
    listingServiceSpy = {
      getById: jest.fn().mockReturnValue(of(mockListing))
    };

    await TestBed.configureTestingModule({
      imports: [ListingDetailComponent, TranslateModule.forRoot()],
      providers: [
        { provide: ListingService, useValue: listingServiceSpy },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: (key: string) => key === 'id' ? 'l1' : null
              }
            }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ListingDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load listing on init', () => {
    expect(listingServiceSpy.getById).toHaveBeenCalledWith('l1');
    expect(component.listing).toEqual(mockListing);
  });

  it('should display listing title', () => {
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('h1')?.textContent).toContain('Nice Car');
  });

  it('should display category name', () => {
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('.category')?.textContent).toContain('Automobiles');
  });

  it('should display region name', () => {
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('.region')?.textContent).toContain('Zurich');
  });

  it('should display attributes', () => {
    const el = fixture.nativeElement as HTMLElement;
    const attrRows = el.querySelectorAll('.attr-row');
    expect(attrRows.length).toBe(1);
    expect(attrRows[0].querySelector('.attr-name')?.textContent).toContain('Brand');
    expect(attrRows[0].querySelector('.attr-value')?.textContent).toContain('BMW');
  });

  it('should display media images', () => {
    const el = fixture.nativeElement as HTMLElement;
    const imgs = el.querySelectorAll('.listing-gallery img');
    expect(imgs.length).toBe(1);
  });

  it('should display contact details', () => {
    const el = fixture.nativeElement as HTMLElement;
    const contact = el.querySelector('.contact');
    expect(contact?.textContent).toContain('123 Main St');
    expect(contact?.textContent).toContain('+41 123 456');
  });

  it('should not load listing when no id in route', async () => {
    TestBed.resetTestingModule();
    listingServiceSpy.getById = jest.fn();

    await TestBed.configureTestingModule({
      imports: [ListingDetailComponent, TranslateModule.forRoot()],
      providers: [
        { provide: ListingService, useValue: listingServiceSpy },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: { get: () => null }
            }
          }
        }
      ]
    }).compileComponents();

    const fix = TestBed.createComponent(ListingDetailComponent);
    fix.detectChanges();
    expect(listingServiceSpy.getById).not.toHaveBeenCalled();
  });
});
