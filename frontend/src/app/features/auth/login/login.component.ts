import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslateModule],
  template: `
    <div class="auth-container">
      <h2>{{ 'auth.login' | translate }}</h2>
      <form (ngSubmit)="onSubmit()">
        <div class="form-group">
          <label>{{ 'auth.email' | translate }}</label>
          <input type="email" [(ngModel)]="email" name="email" required />
        </div>
        <div class="form-group">
          <label>{{ 'auth.password' | translate }}</label>
          <input type="password" [(ngModel)]="password" name="password" required />
        </div>
        <div class="error" *ngIf="error">{{ error }}</div>
        <button type="submit" [disabled]="loading">{{ 'auth.login' | translate }}</button>
        <p><a routerLink="/auth/forgot-password">{{ 'auth.forgotPassword' | translate }}</a></p>
        <p>{{ 'auth.noAccount' | translate }} <a routerLink="/auth/register">{{ 'auth.register' | translate }}</a></p>
      </form>
    </div>
  `
})
export class LoginComponent {
  email = '';
  password = '';
  error = '';
  loading = false;

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit(): void {
    this.loading = true;
    this.error = '';
    this.authService.login({ email: this.email, password: this.password }).subscribe({
      next: () => { this.router.navigate(['/member/dashboard']); },
      error: (err) => { this.error = err.error?.message || 'Login failed'; this.loading = false; }
    });
  }
}
