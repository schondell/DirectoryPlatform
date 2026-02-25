import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { UserService } from '../../../core/services/user.service';
import { User } from '../../../core/models';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  template: `
    <h1>{{ 'nav.profile' | translate }}</h1>
    <form *ngIf="user" (ngSubmit)="onSave()">
      <div class="form-group"><label>{{ 'auth.firstName' | translate }}</label><input [(ngModel)]="user.firstName" name="firstName" /></div>
      <div class="form-group"><label>{{ 'auth.lastName' | translate }}</label><input [(ngModel)]="user.lastName" name="lastName" /></div>
      <div class="form-group"><label>{{ 'listing.phone' | translate }}</label><input [(ngModel)]="user.phone" name="phone" /></div>
      <button type="submit">{{ 'common.save' | translate }}</button>
    </form>
  `
})
export class ProfileComponent implements OnInit {
  user: User | null = null;
  constructor(private userService: UserService) {}
  ngOnInit(): void { this.userService.getProfile().subscribe(u => this.user = u); }
  onSave(): void { if (this.user) this.userService.updateProfile(this.user).subscribe(u => this.user = u); }
}
