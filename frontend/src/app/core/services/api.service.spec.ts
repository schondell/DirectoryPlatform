import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ApiService } from './api.service';
import { environment } from '../../../environments/environment';

describe('ApiService', () => {
  let service: ApiService;
  let httpMock: HttpTestingController;
  const baseUrl = environment.apiUrl;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        ApiService
      ]
    });
    service = TestBed.inject(ApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('get', () => {
    it('should make GET request to correct URL', () => {
      service.get<{ id: string }>('test/path').subscribe(res => {
        expect(res).toEqual({ id: '1' });
      });
      const req = httpMock.expectOne(`${baseUrl}/test/path`);
      expect(req.request.method).toBe('GET');
      req.flush({ id: '1' });
    });

    it('should append query params excluding null/undefined/empty', () => {
      service.get<any>('items', { page: 1, search: 'hello', empty: '', nothing: null, undef: undefined }).subscribe();
      const req = httpMock.expectOne(r => r.url === `${baseUrl}/items`);
      expect(req.request.params.get('page')).toBe('1');
      expect(req.request.params.get('search')).toBe('hello');
      expect(req.request.params.has('empty')).toBe(false);
      expect(req.request.params.has('nothing')).toBe(false);
      expect(req.request.params.has('undef')).toBe(false);
      req.flush([]);
    });

    it('should work without params', () => {
      service.get<any>('data').subscribe();
      const req = httpMock.expectOne(`${baseUrl}/data`);
      expect(req.request.params.keys().length).toBe(0);
      req.flush({});
    });
  });

  describe('post', () => {
    it('should make POST request with body', () => {
      const body = { name: 'test' };
      service.post<any>('create', body).subscribe(res => {
        expect(res).toEqual({ id: '1', name: 'test' });
      });
      const req = httpMock.expectOne(`${baseUrl}/create`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(body);
      req.flush({ id: '1', name: 'test' });
    });
  });

  describe('put', () => {
    it('should make PUT request with body', () => {
      const body = { name: 'updated' };
      service.put<any>('update/1', body).subscribe(res => {
        expect(res).toEqual({ id: '1', name: 'updated' });
      });
      const req = httpMock.expectOne(`${baseUrl}/update/1`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(body);
      req.flush({ id: '1', name: 'updated' });
    });
  });

  describe('delete', () => {
    it('should make DELETE request', () => {
      service.delete<void>('items/1').subscribe();
      const req = httpMock.expectOne(`${baseUrl}/items/1`);
      expect(req.request.method).toBe('DELETE');
      req.flush(null);
    });
  });
});
