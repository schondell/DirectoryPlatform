import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { NotificationService } from '../../../core/services/notification.service';
import { Notification } from '../../../core/models';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  template: `
    <h1>{{ 'nav.notifications' | translate }}</h1>
    <button (click)="markAllRead()">Mark all read</button>
    @for (n of notifications; track n.id) {
      <div class="notification-row" [class.unread]="!n.isRead">
        <strong>{{ n.title }}</strong>
        <p>{{ n.message }}</p>
        <span>{{ n.createdAt | date:'short' }}</span>
      </div>
    }
  `
})
export class NotificationsComponent implements OnInit {
  notifications: Notification[] = [];
  constructor(private notificationService: NotificationService) {}
  ngOnInit(): void { this.notificationService.getAll().subscribe(n => this.notifications = n); }
  markAllRead(): void { this.notificationService.markAllAsRead().subscribe(() => this.notifications.forEach(n => n.isRead = true)); }
}
