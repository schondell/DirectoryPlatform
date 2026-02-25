import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { User } from '../models';

@Injectable({ providedIn: 'root' })
export class UserService {
  constructor(private api: ApiService) {}

  getProfile(): Observable<User> {
    return this.api.get<User>('me');
  }

  updateProfile(dto: any): Observable<User> {
    return this.api.put<User>('me', dto);
  }

  getAll(): Observable<User[]> {
    return this.api.get<User[]>('users');
  }

  getById(id: string): Observable<User> {
    return this.api.get<User>(`users/${id}`);
  }

  updateRole(id: string, role: string): Observable<User> {
    return this.api.put<User>(`users/${id}/role`, JSON.stringify(role));
  }

  toggleLock(id: string): Observable<User> {
    return this.api.put<User>(`users/${id}/toggle-lock`, {});
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`users/${id}`);
  }
}
