import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslateModule],
  template: `
    <div class="auth-container">
      <h2>{{ 'auth.register' | translate }}</h2>
      <form (ngSubmit)="onSubmit()">
        <div class="form-group"><label>{{ 'auth.username' | translate }}</label><input type="text" [(ngModel)]="username" name="username" required /></div>
        <div class="form-group"><label>{{ 'auth.email' | translate }}</label><input type="email" [(ngModel)]="email" name="email" required /></div>
        <div class="form-group"><label>{{ 'auth.password' | translate }}</label><input type="password" [(ngModel)]="password" name="password" required /></div>
        <div class="form-group"><label>{{ 'auth.confirmPassword' | translate }}</label><input type="password" [(ngModel)]="confirmPassword" name="confirmPassword" required /></div>
        <div class="error" *ngIf="error">{{ error }}</div>
        <button type="submit" [disabled]="loading">{{ 'auth.register' | translate }}</button>
        <p>{{ 'auth.hasAccount' | translate }} <a routerLink="/auth/login">{{ 'auth.login' | translate }}</a></p>
      </form>
    </div>
  `
})
export class RegisterComponent {
  username = ''; email = ''; password = ''; confirmPassword = '';
  error = ''; loading = false;

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit(): void {
    this.loading = true; this.error = '';
    this.authService.register({ username: this.username, email: this.email, password: this.password, confirmPassword: this.confirmPassword }).subscribe({
      next: () => this.router.navigate(['/member/dashboard']),
      error: (err) => { this.error = err.error?.message || 'Registration failed'; this.loading = false; }
    });
  }
}
