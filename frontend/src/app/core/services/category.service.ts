import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Category, CategoryWithChildren, AttributeDefinition } from '../models';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  constructor(private api: ApiService) {}

  getAll(): Observable<Category[]> {
    return this.api.get<Category[]>('categories');
  }

  getTree(): Observable<CategoryWithChildren[]> {
    return this.api.get<CategoryWithChildren[]>('categories/tree');
  }

  getById(id: string): Observable<CategoryWithChildren> {
    return this.api.get<CategoryWithChildren>(`categories/${id}`);
  }

  getBySlug(slug: string): Observable<Category> {
    return this.api.get<Category>(`categories/slug/${slug}`);
  }

  getAttributes(categoryId: string, filterableOnly = false): Observable<AttributeDefinition[]> {
    return this.api.get<AttributeDefinition[]>(`categories/${categoryId}/attributes`, { filterableOnly });
  }

  create(dto: any): Observable<Category> {
    return this.api.post<Category>('categories', dto);
  }

  update(id: string, dto: any): Observable<Category> {
    return this.api.put<Category>(`categories/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`categories/${id}`);
  }
}
