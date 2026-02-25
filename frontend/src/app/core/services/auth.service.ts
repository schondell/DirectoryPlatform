import { Injectable, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { ApiService } from './api.service';
import { AuthResponse, LoginRequest, RegisterRequest, User } from '../models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private tokenSignal = signal<string | null>(this.getStoredToken());
  private userSignal = signal<AuthResponse | null>(this.getStoredUser());

  isAuthenticated = computed(() => !!this.tokenSignal());
  currentUser = computed(() => this.userSignal());
  userRole = computed(() => this.userSignal()?.role ?? '');
  userId = computed(() => this.userSignal()?.userId ?? '');

  constructor(private api: ApiService, private router: Router) {}

  login(dto: LoginRequest): Observable<AuthResponse> {
    return this.api.post<AuthResponse>('auth/login', dto).pipe(
      tap(res => this.setSession(res))
    );
  }

  register(dto: RegisterRequest): Observable<AuthResponse> {
    return this.api.post<AuthResponse>('auth/register', dto).pipe(
      tap(res => this.setSession(res))
    );
  }

  refreshToken(): Observable<AuthResponse> {
    const user = this.userSignal();
    return this.api.post<AuthResponse>('auth/refresh-token', {
      token: this.tokenSignal(),
      refreshToken: user?.refreshToken
    }).pipe(tap(res => this.setSession(res)));
  }

  logout(): void {
    this.api.post('auth/logout', {}).subscribe({ error: () => {} });
    this.clearSession();
    this.router.navigate(['/auth/login']);
  }

  getToken(): string | null {
    return this.tokenSignal();
  }

  private setSession(res: AuthResponse): void {
    localStorage.setItem('token', res.token);
    localStorage.setItem('user', JSON.stringify(res));
    this.tokenSignal.set(res.token);
    this.userSignal.set(res);
  }

  private clearSession(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.tokenSignal.set(null);
    this.userSignal.set(null);
  }

  private getStoredToken(): string | null {
    if (typeof window === 'undefined') return null;
    return localStorage.getItem('token');
  }

  private getStoredUser(): AuthResponse | null {
    if (typeof window === 'undefined') return null;
    const data = localStorage.getItem('user');
    return data ? JSON.parse(data) : null;
  }
}
