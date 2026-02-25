import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { AttributeDefinition } from '../models';

@Injectable({ providedIn: 'root' })
export class AttributeDefinitionService {
  constructor(private api: ApiService) {}

  getByCategory(categoryId: string, filterableOnly = false): Observable<AttributeDefinition[]> {
    return this.api.get<AttributeDefinition[]>(`attributedefinitions/category/${categoryId}`, { filterableOnly });
  }

  getById(id: string): Observable<AttributeDefinition> {
    return this.api.get<AttributeDefinition>(`attributedefinitions/${id}`);
  }

  create(dto: any): Observable<AttributeDefinition> {
    return this.api.post<AttributeDefinition>('attributedefinitions', dto);
  }

  update(id: string, dto: any): Observable<AttributeDefinition> {
    return this.api.put<AttributeDefinition>(`attributedefinitions/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`attributedefinitions/${id}`);
  }
}
