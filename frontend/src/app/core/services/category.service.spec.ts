import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { CategoryService } from './category.service';
import { ApiService } from './api.service';
import { Category, CategoryWithChildren, AttributeDefinition } from '../models';

describe('CategoryService', () => {
  let service: CategoryService;
  let apiSpy: jest.Mocked<ApiService>;

  const mockCategory: Category = {
    id: 'c1', name: 'Automobiles', slug: 'automobiles',
    displayOrder: 1
  };

  const mockCategoryWithChildren: CategoryWithChildren = {
    ...mockCategory,
    children: [
      { id: 'c2', name: 'Cars', slug: 'cars', parentId: 'c1', displayOrder: 1, children: [] }
    ]
  };

  beforeEach(() => {
    const api = { get: jest.fn(), post: jest.fn(), put: jest.fn(), delete: jest.fn() };
    TestBed.configureTestingModule({
      providers: [CategoryService, { provide: ApiService, useValue: api }]
    });
    service = TestBed.inject(CategoryService);
    apiSpy = TestBed.inject(ApiService) as jest.Mocked<ApiService>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getAll should call api.get with categories', (done) => {
    apiSpy.get.mockReturnValue(of([mockCategory]));
    service.getAll().subscribe(res => {
      expect(res).toEqual([mockCategory]);
      expect(apiSpy.get).toHaveBeenCalledWith('categories');
      done();
    });
  });

  it('getTree should call api.get with categories/tree', (done) => {
    apiSpy.get.mockReturnValue(of([mockCategoryWithChildren]));
    service.getTree().subscribe(res => {
      expect(res[0].children.length).toBe(1);
      expect(apiSpy.get).toHaveBeenCalledWith('categories/tree');
      done();
    });
  });

  it('getById should call api.get with category id', (done) => {
    apiSpy.get.mockReturnValue(of(mockCategoryWithChildren));
    service.getById('c1').subscribe(res => {
      expect(res.id).toBe('c1');
      expect(apiSpy.get).toHaveBeenCalledWith('categories/c1');
      done();
    });
  });

  it('getBySlug should call api.get with slug', (done) => {
    apiSpy.get.mockReturnValue(of(mockCategory));
    service.getBySlug('automobiles').subscribe(res => {
      expect(res.slug).toBe('automobiles');
      expect(apiSpy.get).toHaveBeenCalledWith('categories/slug/automobiles');
      done();
    });
  });

  it('getAttributes should call api.get with filterableOnly param', (done) => {
    const mockAttrs: AttributeDefinition[] = [
      { id: 'a1', name: 'Brand', slug: 'brand', type: 'Select', categoryId: 'c1', isFilterable: true, isRequired: false, displayOrder: 1 }
    ];
    apiSpy.get.mockReturnValue(of(mockAttrs));
    service.getAttributes('c1', true).subscribe(res => {
      expect(res).toEqual(mockAttrs);
      expect(apiSpy.get).toHaveBeenCalledWith('categories/c1/attributes', { filterableOnly: true });
      done();
    });
  });

  it('getAttributes should default filterableOnly to false', (done) => {
    apiSpy.get.mockReturnValue(of([]));
    service.getAttributes('c1').subscribe(() => {
      expect(apiSpy.get).toHaveBeenCalledWith('categories/c1/attributes', { filterableOnly: false });
      done();
    });
  });

  it('create should call api.post', (done) => {
    apiSpy.post.mockReturnValue(of(mockCategory));
    service.create({ name: 'New' }).subscribe(res => {
      expect(apiSpy.post).toHaveBeenCalledWith('categories', { name: 'New' });
      done();
    });
  });

  it('update should call api.put with id', (done) => {
    apiSpy.put.mockReturnValue(of(mockCategory));
    service.update('c1', { name: 'Updated' }).subscribe(() => {
      expect(apiSpy.put).toHaveBeenCalledWith('categories/c1', { name: 'Updated' });
      done();
    });
  });

  it('delete should call api.delete with id', (done) => {
    apiSpy.delete.mockReturnValue(of(undefined));
    service.delete('c1').subscribe(() => {
      expect(apiSpy.delete).toHaveBeenCalledWith('categories/c1');
      done();
    });
  });
});
