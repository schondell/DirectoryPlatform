import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Message } from '../models';

@Injectable({ providedIn: 'root' })
export class MessageService {
  constructor(private api: ApiService) {}

  getInbox(): Observable<Message[]> {
    return this.api.get<Message[]>('messages/inbox');
  }

  getSent(): Observable<Message[]> {
    return this.api.get<Message[]>('messages/sent');
  }

  getById(id: string): Observable<Message> {
    return this.api.get<Message>(`messages/${id}`);
  }

  getUnreadCount(): Observable<number> {
    return this.api.get<number>('messages/unread-count');
  }

  send(dto: any): Observable<Message> {
    return this.api.post<Message>('messages', dto);
  }

  markAsRead(id: string): Observable<void> {
    return this.api.put<void>(`messages/${id}/read`, {});
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`messages/${id}`);
  }
}
