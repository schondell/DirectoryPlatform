import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { NotificationService } from './notification.service';
import { ApiService } from './api.service';
import { Notification } from '../models';

describe('NotificationService', () => {
  let service: NotificationService;
  let apiSpy: jest.Mocked<ApiService>;

  const mockNotification: Notification = {
    id: 'n1',
    type: 'message',
    title: 'New Message',
    message: 'You have a new message',
    isRead: false,
    createdAt: '2026-01-01'
  };

  beforeEach(() => {
    const api = { get: jest.fn(), post: jest.fn(), put: jest.fn(), delete: jest.fn() };
    TestBed.configureTestingModule({
      providers: [NotificationService, { provide: ApiService, useValue: api }]
    });
    service = TestBed.inject(NotificationService);
    apiSpy = TestBed.inject(ApiService) as jest.Mocked<ApiService>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getAll should call api.get with notifications', (done) => {
    apiSpy.get.mockReturnValue(of([mockNotification]));
    service.getAll().subscribe(res => {
      expect(res).toEqual([mockNotification]);
      expect(apiSpy.get).toHaveBeenCalledWith('notifications');
      done();
    });
  });

  it('getUnreadCount should return count', (done) => {
    apiSpy.get.mockReturnValue(of(3));
    service.getUnreadCount().subscribe(res => {
      expect(res).toBe(3);
      expect(apiSpy.get).toHaveBeenCalledWith('notifications/unread-count');
      done();
    });
  });

  it('markAsRead should call api.put with id', (done) => {
    apiSpy.put.mockReturnValue(of(undefined));
    service.markAsRead('n1').subscribe(() => {
      expect(apiSpy.put).toHaveBeenCalledWith('notifications/n1/read', {});
      done();
    });
  });

  it('markAllAsRead should call api.put for read-all', (done) => {
    apiSpy.put.mockReturnValue(of(undefined));
    service.markAllAsRead().subscribe(() => {
      expect(apiSpy.put).toHaveBeenCalledWith('notifications/read-all', {});
      done();
    });
  });
});
