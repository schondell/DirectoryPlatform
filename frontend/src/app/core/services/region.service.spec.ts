import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { RegionService } from './region.service';
import { ApiService } from './api.service';
import { Region, RegionWithChildren } from '../models';

describe('RegionService', () => {
  let service: RegionService;
  let apiSpy: jest.Mocked<ApiService>;

  const mockRegion: Region = {
    id: 'r1', name: 'Zurich', slug: 'zurich', displayOrder: 1, countryCode: 'CH'
  };

  const mockRegionWithChildren: RegionWithChildren = {
    ...mockRegion,
    children: [
      { id: 'r2', name: 'Winterthur', slug: 'winterthur', parentId: 'r1', displayOrder: 1, children: [] }
    ]
  };

  beforeEach(() => {
    const api = { get: jest.fn(), post: jest.fn(), put: jest.fn(), delete: jest.fn() };
    TestBed.configureTestingModule({
      providers: [RegionService, { provide: ApiService, useValue: api }]
    });
    service = TestBed.inject(RegionService);
    apiSpy = TestBed.inject(ApiService) as jest.Mocked<ApiService>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getAll should call api.get with regions', (done) => {
    apiSpy.get.mockReturnValue(of([mockRegion]));
    service.getAll().subscribe(res => {
      expect(res).toEqual([mockRegion]);
      expect(apiSpy.get).toHaveBeenCalledWith('regions');
      done();
    });
  });

  it('getTree should call api.get with regions/tree', (done) => {
    apiSpy.get.mockReturnValue(of([mockRegionWithChildren]));
    service.getTree().subscribe(res => {
      expect(res[0].children.length).toBe(1);
      expect(apiSpy.get).toHaveBeenCalledWith('regions/tree');
      done();
    });
  });

  it('getById should call api.get with region id', (done) => {
    apiSpy.get.mockReturnValue(of(mockRegionWithChildren));
    service.getById('r1').subscribe(res => {
      expect(res.id).toBe('r1');
      expect(apiSpy.get).toHaveBeenCalledWith('regions/r1');
      done();
    });
  });

  it('create should call api.post', (done) => {
    apiSpy.post.mockReturnValue(of(mockRegion));
    service.create({ name: 'Bern' }).subscribe(() => {
      expect(apiSpy.post).toHaveBeenCalledWith('regions', { name: 'Bern' });
      done();
    });
  });

  it('update should call api.put with id', (done) => {
    apiSpy.put.mockReturnValue(of(mockRegion));
    service.update('r1', { name: 'Updated' }).subscribe(() => {
      expect(apiSpy.put).toHaveBeenCalledWith('regions/r1', { name: 'Updated' });
      done();
    });
  });

  it('delete should call api.delete with id', (done) => {
    apiSpy.delete.mockReturnValue(of(undefined));
    service.delete('r1').subscribe(() => {
      expect(apiSpy.delete).toHaveBeenCalledWith('regions/r1');
      done();
    });
  });
});
