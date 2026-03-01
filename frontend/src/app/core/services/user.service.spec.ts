import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { UserService } from './user.service';
import { ApiService } from './api.service';
import { User } from '../models';

describe('UserService', () => {
  let service: UserService;
  let apiSpy: jest.Mocked<ApiService>;

  const mockUser: User = {
    id: 'u1',
    username: 'testuser',
    email: 'test@test.com',
    role: 'User',
    isEmailVerified: true,
    createdAt: '2026-01-01'
  };

  beforeEach(() => {
    const api = { get: jest.fn(), post: jest.fn(), put: jest.fn(), delete: jest.fn() };
    TestBed.configureTestingModule({
      providers: [UserService, { provide: ApiService, useValue: api }]
    });
    service = TestBed.inject(UserService);
    apiSpy = TestBed.inject(ApiService) as jest.Mocked<ApiService>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getProfile should call api.get with me', (done) => {
    apiSpy.get.mockReturnValue(of(mockUser));
    service.getProfile().subscribe(res => {
      expect(res).toEqual(mockUser);
      expect(apiSpy.get).toHaveBeenCalledWith('me');
      done();
    });
  });

  it('updateProfile should call api.put with me', (done) => {
    apiSpy.put.mockReturnValue(of(mockUser));
    service.updateProfile({ firstName: 'John' }).subscribe(() => {
      expect(apiSpy.put).toHaveBeenCalledWith('me', { firstName: 'John' });
      done();
    });
  });

  it('getAll should call api.get with users', (done) => {
    apiSpy.get.mockReturnValue(of([mockUser]));
    service.getAll().subscribe(res => {
      expect(res).toEqual([mockUser]);
      expect(apiSpy.get).toHaveBeenCalledWith('users');
      done();
    });
  });

  it('getById should call api.get with user id', (done) => {
    apiSpy.get.mockReturnValue(of(mockUser));
    service.getById('u1').subscribe(res => {
      expect(res.id).toBe('u1');
      expect(apiSpy.get).toHaveBeenCalledWith('users/u1');
      done();
    });
  });

  it('updateRole should call api.put with stringified role', (done) => {
    apiSpy.put.mockReturnValue(of(mockUser));
    service.updateRole('u1', 'Admin').subscribe(() => {
      expect(apiSpy.put).toHaveBeenCalledWith('users/u1/role', JSON.stringify('Admin'));
      done();
    });
  });

  it('toggleLock should call api.put with toggle-lock', (done) => {
    apiSpy.put.mockReturnValue(of(mockUser));
    service.toggleLock('u1').subscribe(() => {
      expect(apiSpy.put).toHaveBeenCalledWith('users/u1/toggle-lock', {});
      done();
    });
  });

  it('delete should call api.delete with id', (done) => {
    apiSpy.delete.mockReturnValue(of(undefined));
    service.delete('u1').subscribe(() => {
      expect(apiSpy.delete).toHaveBeenCalledWith('users/u1');
      done();
    });
  });
});
