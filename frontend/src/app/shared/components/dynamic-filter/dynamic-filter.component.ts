import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AttributeDefinitionService } from '../../../core/services/attribute-definition.service';
import { AttributeDefinition } from '../../../core/models';

@Component({
  selector: 'app-dynamic-filter',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="dynamic-filters" *ngIf="attributes.length">
      @for (attr of attributes; track attr.id) {
        <div class="filter-control">
          <label>{{ attr.name }}{{ attr.unit ? ' (' + attr.unit + ')' : '' }}</label>

          @switch (attr.type) {
            @case ('Boolean') {
              <label class="toggle">
                <input type="checkbox" [ngModel]="filterValues[attr.slug] === 'true'" (ngModelChange)="onBooleanChange(attr.slug, $event)" />
                {{ attr.name }}
              </label>
            }
            @case ('Select') {
              <select [ngModel]="filterValues[attr.slug] || ''" (ngModelChange)="onFilterChange(attr.slug, $event)">
                <option value="">All</option>
                @for (opt of attr.options; track opt) {
                  <option [value]="opt">{{ opt }}</option>
                }
              </select>
            }
            @case ('MultiSelect') {
              <select [ngModel]="filterValues[attr.slug] || ''" (ngModelChange)="onFilterChange(attr.slug, $event)">
                <option value="">All</option>
                @for (opt of attr.options; track opt) {
                  <option [value]="opt">{{ opt }}</option>
                }
              </select>
            }
            @case ('Number') {
              <div class="range-inputs">
                <input type="number" [placeholder]="'Min' + (attr.minValue !== null ? ' (' + attr.minValue + ')' : '')"
                  [ngModel]="getMin(attr.slug)" (ngModelChange)="onRangeChange(attr.slug, $event, getMax(attr.slug))" />
                <span>-</span>
                <input type="number" [placeholder]="'Max' + (attr.maxValue !== null ? ' (' + attr.maxValue + ')' : '')"
                  [ngModel]="getMax(attr.slug)" (ngModelChange)="onRangeChange(attr.slug, getMin(attr.slug), $event)" />
              </div>
            }
            @case ('Date') {
              <input type="date" [ngModel]="filterValues[attr.slug] || ''" (ngModelChange)="onFilterChange(attr.slug, $event)" />
            }
            @default {
              <input type="text" [ngModel]="filterValues[attr.slug] || ''" (ngModelChange)="onFilterChange(attr.slug, $event)" placeholder="Search..." />
            }
          }
        </div>
      }
    </div>
  `
})
export class DynamicFilterComponent implements OnChanges {
  @Input() categoryId: string = '';
  @Output() filterChanged = new EventEmitter<Record<string, string>>();

  attributes: AttributeDefinition[] = [];
  filterValues: Record<string, string> = {};

  constructor(private attrService: AttributeDefinitionService) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['categoryId'] && this.categoryId) {
      this.filterValues = {};
      this.attrService.getByCategory(this.categoryId, true).subscribe(attrs => this.attributes = attrs);
    }
  }

  onFilterChange(slug: string, value: string): void {
    if (value) { this.filterValues[slug] = value; }
    else { delete this.filterValues[slug]; }
    this.filterChanged.emit({ ...this.filterValues });
  }

  onBooleanChange(slug: string, checked: boolean): void {
    this.onFilterChange(slug, checked ? 'true' : '');
  }

  onRangeChange(slug: string, min: string, max: string): void {
    if (min && max) { this.onFilterChange(slug, `${min}-${max}`); }
    else if (min) { this.onFilterChange(slug, `min:${min}`); }
    else if (max) { this.onFilterChange(slug, `max:${max}`); }
    else { this.onFilterChange(slug, ''); }
  }

  getMin(slug: string): string {
    const val = this.filterValues[slug] || '';
    if (val.includes('-')) return val.split('-')[0];
    if (val.startsWith('min:')) return val.substring(4);
    return '';
  }

  getMax(slug: string): string {
    const val = this.filterValues[slug] || '';
    if (val.includes('-') && !val.startsWith('min:') && !val.startsWith('max:')) return val.split('-')[1];
    if (val.startsWith('max:')) return val.substring(4);
    return '';
  }
}
