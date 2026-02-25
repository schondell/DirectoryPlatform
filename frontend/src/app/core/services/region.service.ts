import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Region, RegionWithChildren } from '../models';

@Injectable({ providedIn: 'root' })
export class RegionService {
  constructor(private api: ApiService) {}

  getAll(): Observable<Region[]> {
    return this.api.get<Region[]>('regions');
  }

  getTree(): Observable<RegionWithChildren[]> {
    return this.api.get<RegionWithChildren[]>('regions/tree');
  }

  getById(id: string): Observable<RegionWithChildren> {
    return this.api.get<RegionWithChildren>(`regions/${id}`);
  }

  create(dto: any): Observable<Region> {
    return this.api.post<Region>('regions', dto);
  }

  update(id: string, dto: any): Observable<Region> {
    return this.api.put<Region>(`regions/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`regions/${id}`);
  }
}
