import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Notification } from '../models';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  constructor(private api: ApiService) {}

  getAll(): Observable<Notification[]> {
    return this.api.get<Notification[]>('notifications');
  }

  getUnreadCount(): Observable<number> {
    return this.api.get<number>('notifications/unread-count');
  }

  markAsRead(id: string): Observable<void> {
    return this.api.put<void>(`notifications/${id}/read`, {});
  }

  markAllAsRead(): Observable<void> {
    return this.api.put<void>('notifications/read-all', {});
  }
}
