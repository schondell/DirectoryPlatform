import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  template: `
    <div class="auth-container">
      <h2>{{ 'auth.forgotPassword' | translate }}</h2>
      <form (ngSubmit)="onSubmit()" *ngIf="!sent">
        <div class="form-group"><label>{{ 'auth.email' | translate }}</label><input type="email" [(ngModel)]="email" name="email" required /></div>
        <button type="submit">{{ 'common.submit' | translate }}</button>
      </form>
      <p *ngIf="sent">A reset link has been sent if the email exists.</p>
    </div>
  `
})
export class ForgotPasswordComponent {
  email = ''; sent = false;
  constructor(private api: ApiService) {}
  onSubmit(): void {
    this.api.post('auth/forgot-password', { email: this.email }).subscribe({ next: () => this.sent = true, error: () => this.sent = true });
  }
}
