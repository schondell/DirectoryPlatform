import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { CategoryService } from '../../../core/services/category.service';
import { CategoryWithChildren } from '../../../core/models';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  template: `
    <h1>{{ 'admin.categories' | translate }}</h1>
    @for (cat of categories; track cat.id) {
      <div class="category-row">
        <strong>{{ cat.name }}</strong> ({{ cat.slug }})
        @for (child of cat.children; track child.id) {
          <div class="child-row">&nbsp;&nbsp;{{ child.name }} ({{ child.slug }})</div>
        }
      </div>
    }
  `
})
export class AdminCategoriesComponent implements OnInit {
  categories: CategoryWithChildren[] = [];
  constructor(private categoryService: CategoryService) {}
  ngOnInit(): void { this.categoryService.getTree().subscribe(c => this.categories = c); }
}
