import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { AuthService } from './auth.service';
import { ApiService } from './api.service';
import { AuthResponse } from '../models';

describe('AuthService', () => {
  let service: AuthService;
  let apiSpy: jest.Mocked<ApiService>;
  let routerSpy: jest.Mocked<Router>;

  const mockAuthResponse: AuthResponse = {
    token: 'test-token',
    refreshToken: 'test-refresh-token',
    expiration: '2026-12-31T00:00:00Z',
    username: 'testuser',
    email: 'test@test.com',
    role: 'User',
    userId: 'user-1'
  };

  beforeEach(() => {
    const api = {
      get: jest.fn(),
      post: jest.fn(),
      put: jest.fn(),
      delete: jest.fn()
    };
    const router = {
      navigate: jest.fn()
    };

    // Clear localStorage before each test
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        { provide: ApiService, useValue: api },
        { provide: Router, useValue: router }
      ]
    });

    service = TestBed.inject(AuthService);
    apiSpy = TestBed.inject(ApiService) as jest.Mocked<ApiService>;
    routerSpy = TestBed.inject(Router) as jest.Mocked<Router>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should not be authenticated initially when localStorage is empty', () => {
    expect(service.isAuthenticated()).toBe(false);
    expect(service.currentUser()).toBeNull();
    expect(service.userRole()).toBe('');
    expect(service.userId()).toBe('');
  });

  describe('login', () => {
    it('should call api.post and store session on success', (done) => {
      apiSpy.post.mockReturnValue(of(mockAuthResponse));

      service.login({ email: 'test@test.com', password: 'pass123' }).subscribe(res => {
        expect(res).toEqual(mockAuthResponse);
        expect(apiSpy.post).toHaveBeenCalledWith('auth/login', { email: 'test@test.com', password: 'pass123' });
        expect(localStorage.getItem('token')).toBe('test-token');
        expect(JSON.parse(localStorage.getItem('user')!)).toEqual(mockAuthResponse);
        expect(service.isAuthenticated()).toBe(true);
        expect(service.currentUser()).toEqual(mockAuthResponse);
        expect(service.userRole()).toBe('User');
        expect(service.userId()).toBe('user-1');
        done();
      });
    });
  });

  describe('register', () => {
    it('should call api.post and store session on success', (done) => {
      apiSpy.post.mockReturnValue(of(mockAuthResponse));

      service.register({
        username: 'testuser',
        email: 'test@test.com',
        password: 'pass123',
        confirmPassword: 'pass123'
      }).subscribe(res => {
        expect(res).toEqual(mockAuthResponse);
        expect(apiSpy.post).toHaveBeenCalledWith('auth/register', {
          username: 'testuser',
          email: 'test@test.com',
          password: 'pass123',
          confirmPassword: 'pass123'
        });
        expect(service.isAuthenticated()).toBe(true);
        done();
      });
    });
  });

  describe('refreshToken', () => {
    it('should call api.post with current token and refresh token', (done) => {
      // First login to set the session
      apiSpy.post.mockReturnValue(of(mockAuthResponse));
      service.login({ email: 'test@test.com', password: 'pass' }).subscribe();

      const newAuth: AuthResponse = { ...mockAuthResponse, token: 'new-token' };
      apiSpy.post.mockReturnValue(of(newAuth));

      service.refreshToken().subscribe(res => {
        expect(res.token).toBe('new-token');
        expect(service.getToken()).toBe('new-token');
        done();
      });
    });
  });

  describe('logout', () => {
    it('should clear session, call api.post and navigate to login', () => {
      // Set up logged-in state
      apiSpy.post.mockReturnValue(of(mockAuthResponse));
      service.login({ email: 'test@test.com', password: 'pass' }).subscribe();

      // Now logout
      apiSpy.post.mockReturnValue(of({}));
      service.logout();

      expect(service.isAuthenticated()).toBe(false);
      expect(service.getToken()).toBeNull();
      expect(localStorage.getItem('token')).toBeNull();
      expect(localStorage.getItem('user')).toBeNull();
      expect(routerSpy.navigate).toHaveBeenCalledWith(['/auth/login']);
    });
  });

  describe('getToken', () => {
    it('should return null when not authenticated', () => {
      expect(service.getToken()).toBeNull();
    });

    it('should return token when authenticated', (done) => {
      apiSpy.post.mockReturnValue(of(mockAuthResponse));
      service.login({ email: 'e', password: 'p' }).subscribe(() => {
        expect(service.getToken()).toBe('test-token');
        done();
      });
    });
  });
});
