import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { LoginComponent } from './login.component';
import { AuthService } from '../../../core/services/auth.service';
import { TranslateModule } from '@ngx-translate/core';
import { AuthResponse } from '../../../core/models';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authServiceSpy: jest.Mocked<Partial<AuthService>>;
  let router: Router;

  const mockAuthResponse: AuthResponse = {
    token: 'token', refreshToken: 'refresh', expiration: '2026-12-31',
    username: 'testuser', email: 'test@test.com', role: 'User', userId: 'u1'
  };

  beforeEach(async () => {
    authServiceSpy = {
      login: jest.fn()
    };

    await TestBed.configureTestingModule({
      imports: [LoginComponent, TranslateModule.forRoot()],
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: authServiceSpy }
      ]
    }).compileComponents();

    router = TestBed.inject(Router);
    jest.spyOn(router, 'navigate').mockResolvedValue(true);

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have empty initial state', () => {
    expect(component.email).toBe('');
    expect(component.password).toBe('');
    expect(component.error).toBe('');
    expect(component.loading).toBe(false);
  });

  it('should set loading and call authService.login on submit', () => {
    authServiceSpy.login!.mockReturnValue(of(mockAuthResponse));
    component.email = 'test@test.com';
    component.password = 'pass123';
    component.onSubmit();
    expect(component.loading).toBe(true);
    expect(authServiceSpy.login).toHaveBeenCalledWith({ email: 'test@test.com', password: 'pass123' });
  });

  it('should navigate to member dashboard on successful login', () => {
    authServiceSpy.login!.mockReturnValue(of(mockAuthResponse));
    component.email = 'test@test.com';
    component.password = 'pass123';
    component.onSubmit();
    expect(router.navigate).toHaveBeenCalledWith(['/member/dashboard']);
  });

  it('should set error message on login failure', () => {
    authServiceSpy.login!.mockReturnValue(throwError(() => ({ error: { message: 'Invalid credentials' } })));
    component.email = 'bad@test.com';
    component.password = 'wrong';
    component.onSubmit();
    expect(component.error).toBe('Invalid credentials');
    expect(component.loading).toBe(false);
  });

  it('should set default error message when error has no message', () => {
    authServiceSpy.login!.mockReturnValue(throwError(() => ({ error: {} })));
    component.email = 'bad@test.com';
    component.password = 'wrong';
    component.onSubmit();
    expect(component.error).toBe('Login failed');
    expect(component.loading).toBe(false);
  });

  it('should clear error before submitting', () => {
    component.error = 'Previous error';
    authServiceSpy.login!.mockReturnValue(of(mockAuthResponse));
    component.email = 'test@test.com';
    component.password = 'pass';
    component.onSubmit();
    expect(component.error).toBe('');
  });

  it('should render login form', () => {
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('form')).toBeTruthy();
    expect(el.querySelectorAll('input').length).toBe(2);
    expect(el.querySelector('button[type="submit"]')).toBeTruthy();
  });
});
