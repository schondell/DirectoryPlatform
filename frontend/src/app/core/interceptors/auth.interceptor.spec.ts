import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { authInterceptor } from './auth.interceptor';

describe('authInterceptor', () => {
  let httpClient: HttpClient;
  let httpMock: HttpTestingController;
  let authService: {
    getToken: jest.Mock;
    refreshToken: jest.Mock;
    logout: jest.Mock;
  };

  beforeEach(() => {
    authService = {
      getToken: jest.fn(),
      refreshToken: jest.fn(),
      logout: jest.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
        { provide: AuthService, useValue: authService }
      ]
    });

    httpClient = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should add Authorization header when token exists', () => {
    authService.getToken.mockReturnValue('test-token');

    httpClient.get('/api/test').subscribe();

    const req = httpMock.expectOne('/api/test');
    expect(req.request.headers.get('Authorization')).toBe('Bearer test-token');
    req.flush({});
  });

  it('should not add Authorization header when no token', () => {
    authService.getToken.mockReturnValue(null);

    httpClient.get('/api/test').subscribe();

    const req = httpMock.expectOne('/api/test');
    expect(req.request.headers.has('Authorization')).toBe(false);
    req.flush({});
  });

  it('should pass through non-401 errors', () => {
    authService.getToken.mockReturnValue('token');

    let errorResponse: any;
    httpClient.get('/api/test').subscribe({
      error: (err) => { errorResponse = err; }
    });

    const req = httpMock.expectOne('/api/test');
    req.flush('Server Error', { status: 500, statusText: 'Internal Server Error' });

    expect(errorResponse.status).toBe(500);
  });

  it('should not attempt refresh for auth endpoints on 401', () => {
    authService.getToken.mockReturnValue('token');

    let errorResponse: any;
    httpClient.get('/api/auth/login').subscribe({
      error: (err) => { errorResponse = err; }
    });

    const req = httpMock.expectOne('/api/auth/login');
    req.flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });

    expect(authService.refreshToken).not.toHaveBeenCalled();
    expect(errorResponse.status).toBe(401);
  });

  it('should not attempt refresh when no token on 401', () => {
    authService.getToken.mockReturnValue(null);

    let errorResponse: any;
    httpClient.get('/api/test').subscribe({
      error: (err) => { errorResponse = err; }
    });

    const req = httpMock.expectOne('/api/test');
    req.flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });

    expect(authService.refreshToken).not.toHaveBeenCalled();
  });
});
