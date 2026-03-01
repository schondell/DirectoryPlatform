import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { RegisterComponent } from './register.component';
import { AuthService } from '../../../core/services/auth.service';
import { TranslateModule } from '@ngx-translate/core';
import { AuthResponse } from '../../../core/models';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;
  let authServiceSpy: jest.Mocked<Partial<AuthService>>;
  let router: Router;

  const mockAuthResponse: AuthResponse = {
    token: 'token', refreshToken: 'refresh', expiration: '2026-12-31',
    username: 'newuser', email: 'new@test.com', role: 'User', userId: 'u2'
  };

  beforeEach(async () => {
    authServiceSpy = {
      register: jest.fn()
    };

    await TestBed.configureTestingModule({
      imports: [RegisterComponent, TranslateModule.forRoot()],
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: authServiceSpy }
      ]
    }).compileComponents();

    router = TestBed.inject(Router);
    jest.spyOn(router, 'navigate').mockResolvedValue(true);

    fixture = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have empty initial state', () => {
    expect(component.username).toBe('');
    expect(component.email).toBe('');
    expect(component.password).toBe('');
    expect(component.confirmPassword).toBe('');
    expect(component.error).toBe('');
    expect(component.loading).toBe(false);
  });

  it('should call authService.register on submit', () => {
    authServiceSpy.register!.mockReturnValue(of(mockAuthResponse));
    component.username = 'newuser';
    component.email = 'new@test.com';
    component.password = 'pass123';
    component.confirmPassword = 'pass123';
    component.onSubmit();
    expect(authServiceSpy.register).toHaveBeenCalledWith({
      username: 'newuser',
      email: 'new@test.com',
      password: 'pass123',
      confirmPassword: 'pass123'
    });
  });

  it('should navigate to member dashboard on successful registration', () => {
    authServiceSpy.register!.mockReturnValue(of(mockAuthResponse));
    component.username = 'newuser';
    component.email = 'new@test.com';
    component.password = 'pass123';
    component.confirmPassword = 'pass123';
    component.onSubmit();
    expect(router.navigate).toHaveBeenCalledWith(['/member/dashboard']);
  });

  it('should set error message on registration failure', () => {
    authServiceSpy.register!.mockReturnValue(throwError(() => ({ error: { message: 'Email taken' } })));
    component.username = 'user';
    component.email = 'taken@test.com';
    component.password = 'pass';
    component.confirmPassword = 'pass';
    component.onSubmit();
    expect(component.error).toBe('Email taken');
    expect(component.loading).toBe(false);
  });

  it('should set default error when no message in error response', () => {
    authServiceSpy.register!.mockReturnValue(throwError(() => ({ error: {} })));
    component.onSubmit();
    expect(component.error).toBe('Registration failed');
  });

  it('should render register form with 4 inputs', () => {
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('form')).toBeTruthy();
    expect(el.querySelectorAll('input').length).toBe(4);
  });
});
