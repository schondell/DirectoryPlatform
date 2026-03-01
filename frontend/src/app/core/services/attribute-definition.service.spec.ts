import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { AttributeDefinitionService } from './attribute-definition.service';
import { ApiService } from './api.service';
import { AttributeDefinition } from '../models';

describe('AttributeDefinitionService', () => {
  let service: AttributeDefinitionService;
  let apiSpy: jest.Mocked<ApiService>;

  const mockAttr: AttributeDefinition = {
    id: 'a1',
    name: 'Brand',
    slug: 'brand',
    type: 'Select',
    categoryId: 'c1',
    isFilterable: true,
    isRequired: false,
    displayOrder: 1,
    options: ['BMW', 'Audi', 'Mercedes']
  };

  beforeEach(() => {
    const api = { get: jest.fn(), post: jest.fn(), put: jest.fn(), delete: jest.fn() };
    TestBed.configureTestingModule({
      providers: [AttributeDefinitionService, { provide: ApiService, useValue: api }]
    });
    service = TestBed.inject(AttributeDefinitionService);
    apiSpy = TestBed.inject(ApiService) as jest.Mocked<ApiService>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getByCategory should call api.get with category id and filterableOnly', (done) => {
    apiSpy.get.mockReturnValue(of([mockAttr]));
    service.getByCategory('c1', true).subscribe(res => {
      expect(res).toEqual([mockAttr]);
      expect(apiSpy.get).toHaveBeenCalledWith('attributedefinitions/category/c1', { filterableOnly: true });
      done();
    });
  });

  it('getByCategory should default filterableOnly to false', (done) => {
    apiSpy.get.mockReturnValue(of([]));
    service.getByCategory('c1').subscribe(() => {
      expect(apiSpy.get).toHaveBeenCalledWith('attributedefinitions/category/c1', { filterableOnly: false });
      done();
    });
  });

  it('getById should call api.get with attr id', (done) => {
    apiSpy.get.mockReturnValue(of(mockAttr));
    service.getById('a1').subscribe(res => {
      expect(res.id).toBe('a1');
      expect(apiSpy.get).toHaveBeenCalledWith('attributedefinitions/a1');
      done();
    });
  });

  it('create should call api.post', (done) => {
    apiSpy.post.mockReturnValue(of(mockAttr));
    const dto = { name: 'Color', slug: 'color', type: 'Select', categoryId: 'c1' };
    service.create(dto).subscribe(() => {
      expect(apiSpy.post).toHaveBeenCalledWith('attributedefinitions', dto);
      done();
    });
  });

  it('update should call api.put with id', (done) => {
    apiSpy.put.mockReturnValue(of(mockAttr));
    service.update('a1', { name: 'Updated' }).subscribe(() => {
      expect(apiSpy.put).toHaveBeenCalledWith('attributedefinitions/a1', { name: 'Updated' });
      done();
    });
  });

  it('delete should call api.delete with id', (done) => {
    apiSpy.delete.mockReturnValue(of(undefined));
    service.delete('a1').subscribe(() => {
      expect(apiSpy.delete).toHaveBeenCalledWith('attributedefinitions/a1');
      done();
    });
  });
});
