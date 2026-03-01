import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { MessageService } from './message.service';
import { ApiService } from './api.service';
import { Message } from '../models';

describe('MessageService', () => {
  let service: MessageService;
  let apiSpy: jest.Mocked<ApiService>;

  const mockMessage: Message = {
    id: 'm1',
    senderId: 'u1',
    senderName: 'Alice',
    recipientId: 'u2',
    recipientName: 'Bob',
    subject: 'Hello',
    body: 'Test message',
    isRead: false,
    createdAt: '2026-01-01'
  };

  beforeEach(() => {
    const api = { get: jest.fn(), post: jest.fn(), put: jest.fn(), delete: jest.fn() };
    TestBed.configureTestingModule({
      providers: [MessageService, { provide: ApiService, useValue: api }]
    });
    service = TestBed.inject(MessageService);
    apiSpy = TestBed.inject(ApiService) as jest.Mocked<ApiService>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getInbox should call api.get with messages/inbox', (done) => {
    apiSpy.get.mockReturnValue(of([mockMessage]));
    service.getInbox().subscribe(res => {
      expect(res).toEqual([mockMessage]);
      expect(apiSpy.get).toHaveBeenCalledWith('messages/inbox');
      done();
    });
  });

  it('getSent should call api.get with messages/sent', (done) => {
    apiSpy.get.mockReturnValue(of([mockMessage]));
    service.getSent().subscribe(res => {
      expect(apiSpy.get).toHaveBeenCalledWith('messages/sent');
      done();
    });
  });

  it('getById should call api.get with message id', (done) => {
    apiSpy.get.mockReturnValue(of(mockMessage));
    service.getById('m1').subscribe(res => {
      expect(res.id).toBe('m1');
      expect(apiSpy.get).toHaveBeenCalledWith('messages/m1');
      done();
    });
  });

  it('getUnreadCount should call api.get', (done) => {
    apiSpy.get.mockReturnValue(of(5));
    service.getUnreadCount().subscribe(res => {
      expect(res).toBe(5);
      expect(apiSpy.get).toHaveBeenCalledWith('messages/unread-count');
      done();
    });
  });

  it('send should call api.post with dto', (done) => {
    apiSpy.post.mockReturnValue(of(mockMessage));
    const dto = { recipientId: 'u2', subject: 'Hi', body: 'Hello' };
    service.send(dto).subscribe(res => {
      expect(apiSpy.post).toHaveBeenCalledWith('messages', dto);
      done();
    });
  });

  it('markAsRead should call api.put', (done) => {
    apiSpy.put.mockReturnValue(of(undefined));
    service.markAsRead('m1').subscribe(() => {
      expect(apiSpy.put).toHaveBeenCalledWith('messages/m1/read', {});
      done();
    });
  });

  it('delete should call api.delete', (done) => {
    apiSpy.delete.mockReturnValue(of(undefined));
    service.delete('m1').subscribe(() => {
      expect(apiSpy.delete).toHaveBeenCalledWith('messages/m1');
      done();
    });
  });
});
