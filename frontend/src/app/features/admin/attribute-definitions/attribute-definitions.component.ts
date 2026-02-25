import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { CategoryService } from '../../../core/services/category.service';
import { AttributeDefinitionService } from '../../../core/services/attribute-definition.service';
import { CategoryWithChildren, AttributeDefinition } from '../../../core/models';

@Component({
  selector: 'app-attribute-definitions',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  template: `
    <h1>{{ 'admin.attributes' | translate }}</h1>
    <div class="filter-bar">
      <select [(ngModel)]="selectedCategoryId" (ngModelChange)="onCategoryChange($event)">
        <option value="">Select category</option>
        @for (cat of categories; track cat.id) {
          <option [value]="cat.id">{{ cat.name }}</option>
          @for (child of cat.children; track child.id) {
            <option [value]="child.id">&nbsp;&nbsp;{{ child.name }}</option>
          }
        }
      </select>
    </div>
    @for (attr of attributes; track attr.id) {
      <div class="attr-row">
        <span><strong>{{ attr.name }}</strong> ({{ attr.slug }})</span>
        <span>Type: {{ attr.type }}</span>
        <span *ngIf="attr.unit">Unit: {{ attr.unit }}</span>
        <span>Filterable: {{ attr.isFilterable ? 'Yes' : 'No' }}</span>
        <span>Required: {{ attr.isRequired ? 'Yes' : 'No' }}</span>
        <span *ngIf="attr.options?.length">Options: {{ attr.options?.join(', ') }}</span>
      </div>
    }
  `
})
export class AttributeDefinitionsComponent implements OnInit {
  categories: CategoryWithChildren[] = [];
  attributes: AttributeDefinition[] = [];
  selectedCategoryId = '';
  constructor(private categoryService: CategoryService, private attrService: AttributeDefinitionService) {}
  ngOnInit(): void { this.categoryService.getTree().subscribe(c => this.categories = c); }
  onCategoryChange(id: string): void {
    if (id) { this.attrService.getByCategory(id).subscribe(a => this.attributes = a); }
    else { this.attributes = []; }
  }
}
