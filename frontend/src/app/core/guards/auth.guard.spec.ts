import { TestBed } from '@angular/core/testing';
import { Router, UrlTree } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { authGuard, roleGuard } from './auth.guard';

describe('authGuard', () => {
  let authService: { isAuthenticated: jest.Mock; userRole: jest.Mock };
  let router: { createUrlTree: jest.Mock };

  beforeEach(() => {
    authService = {
      isAuthenticated: jest.fn(),
      userRole: jest.fn()
    };
    router = {
      createUrlTree: jest.fn().mockReturnValue('login-url-tree' as unknown as UrlTree)
    };

    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: authService },
        { provide: Router, useValue: router }
      ]
    });
  });

  it('should return true if user is authenticated', () => {
    authService.isAuthenticated.mockReturnValue(true);
    const result = TestBed.runInInjectionContext(() => authGuard({} as any, {} as any));
    expect(result).toBe(true);
  });

  it('should redirect to login if user is not authenticated', () => {
    authService.isAuthenticated.mockReturnValue(false);
    const result = TestBed.runInInjectionContext(() => authGuard({} as any, {} as any));
    expect(router.createUrlTree).toHaveBeenCalledWith(['/auth/login']);
    expect(result).toBe('login-url-tree');
  });
});

describe('roleGuard', () => {
  let authService: { isAuthenticated: jest.Mock; userRole: jest.Mock };
  let router: { createUrlTree: jest.Mock };

  beforeEach(() => {
    authService = {
      isAuthenticated: jest.fn(),
      userRole: jest.fn()
    };
    router = {
      createUrlTree: jest.fn().mockImplementation((segments: string[]) => `url-tree:${segments[0]}` as unknown as UrlTree)
    };

    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: authService },
        { provide: Router, useValue: router }
      ]
    });
  });

  it('should redirect to login if not authenticated', () => {
    authService.isAuthenticated.mockReturnValue(false);
    const guard = roleGuard(['Admin']);
    const result = TestBed.runInInjectionContext(() => guard({} as any, {} as any));
    expect(router.createUrlTree).toHaveBeenCalledWith(['/auth/login']);
  });

  it('should return true if user has allowed role', () => {
    authService.isAuthenticated.mockReturnValue(true);
    authService.userRole.mockReturnValue('Admin');
    const guard = roleGuard(['Admin', 'SuperAdmin']);
    const result = TestBed.runInInjectionContext(() => guard({} as any, {} as any));
    expect(result).toBe(true);
  });

  it('should redirect to / if user role is not in allowed list', () => {
    authService.isAuthenticated.mockReturnValue(true);
    authService.userRole.mockReturnValue('User');
    const guard = roleGuard(['Admin']);
    const result = TestBed.runInInjectionContext(() => guard({} as any, {} as any));
    expect(router.createUrlTree).toHaveBeenCalledWith(['/']);
  });
});
