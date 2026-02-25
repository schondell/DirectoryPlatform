import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ListingStateService } from '../../../core/state/listing-state.service';
import { CategoryService } from '../../../core/services/category.service';
import { RegionService } from '../../../core/services/region.service';
import { CategoryWithChildren, RegionWithChildren } from '../../../core/models';

@Component({
  selector: 'app-listing-grid',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, TranslateModule],
  template: `
    <div class="listing-grid-page">
      <div class="filters-sidebar">
        <h3>{{ 'listing.filterBy' | translate }}</h3>

        <div class="filter-group">
          <label>{{ 'listing.category' | translate }}</label>
          <select [(ngModel)]="selectedCategoryId" (ngModelChange)="onCategoryChange($event)">
            <option value="">{{ 'common.all' | translate }}</option>
            @for (cat of categories; track cat.id) {
              <option [value]="cat.id">{{ cat.name }}</option>
              @for (child of cat.children; track child.id) {
                <option [value]="child.id">&nbsp;&nbsp;{{ child.name }}</option>
              }
            }
          </select>
        </div>

        <div class="filter-group">
          <label>{{ 'listing.region' | translate }}</label>
          <select [(ngModel)]="selectedRegionId" (ngModelChange)="onRegionChange($event)">
            <option value="">{{ 'common.all' | translate }}</option>
            @for (reg of regions; track reg.id) {
              <option [value]="reg.id">{{ reg.name }}</option>
              @for (child of reg.children; track child.id) {
                <option [value]="child.id">&nbsp;&nbsp;{{ child.name }}</option>
              }
            }
          </select>
        </div>

        <div class="filter-group">
          <label>{{ 'common.search' | translate }}</label>
          <input type="text" [(ngModel)]="searchTerm" (keyup.enter)="onSearch()" [placeholder]="'common.search' | translate" />
        </div>

        <!-- Dynamic filters will be rendered here based on category -->
      </div>

      <div class="listings-content">
        <div class="listings-header">
          <span>{{ state.totalCount() }} {{ 'listing.results' | translate }}</span>
          <select [(ngModel)]="sortBy" (ngModelChange)="onSortChange($event)">
            <option value="">{{ 'listing.sortBy' | translate }}</option>
            <option value="createdat">{{ 'listing.created' | translate }}</option>
            <option value="title">{{ 'listing.title' | translate }}</option>
            <option value="viewcount">{{ 'listing.views' | translate }}</option>
            <option value="weight">Weight</option>
          </select>
        </div>

        @if (state.loading()) {
          <div class="loading">{{ 'common.loading' | translate }}</div>
        }

        <div class="grid">
          @for (listing of state.listings(); track listing.id) {
            <a [routerLink]="['/listings', listing.id]" class="listing-card">
              @if (listing.media?.length) {
                <img [src]="listing.media[0].url" [alt]="listing.title" class="listing-thumb" />
              }
              <div class="listing-info">
                <h3>{{ listing.title }}</h3>
                <p>{{ listing.shortDescription }}</p>
                <div class="listing-meta">
                  <span class="category">{{ listing.category?.name }}</span>
                  @if (listing.region) {
                    <span class="region">{{ listing.region.name }}</span>
                  }
                </div>
                <div class="listing-attrs">
                  @for (attr of listing.attributes?.slice(0, 3); track attr.id) {
                    <span class="attr">{{ attr.attributeName }}: {{ attr.value }}{{ attr.unit ? ' ' + attr.unit : '' }}</span>
                  }
                </div>
              </div>
            </a>
          }
        </div>

        @if (!state.loading() && state.listings().length === 0) {
          <div class="no-results">{{ 'common.noResults' | translate }}</div>
        }

        <div class="pagination" *ngIf="state.totalPages() > 1">
          <button (click)="state.setPage(state.pageNumber() - 1)" [disabled]="!state.hasPreviousPage()">
            {{ 'common.previous' | translate }}
          </button>
          <span>{{ state.pageNumber() }} / {{ state.totalPages() }}</span>
          <button (click)="state.setPage(state.pageNumber() + 1)" [disabled]="!state.hasNextPage()">
            {{ 'common.next' | translate }}
          </button>
        </div>
      </div>
    </div>
  `
})
export class ListingGridComponent implements OnInit {
  state = inject(ListingStateService);
  categories: CategoryWithChildren[] = [];
  regions: RegionWithChildren[] = [];
  selectedCategoryId = '';
  selectedRegionId = '';
  searchTerm = '';
  sortBy = '';

  constructor(
    private categoryService: CategoryService,
    private regionService: RegionService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.categoryService.getTree().subscribe(c => this.categories = c);
    this.regionService.getTree().subscribe(r => this.regions = r);

    this.route.queryParams.subscribe(params => {
      if (params['categoryId']) {
        this.selectedCategoryId = params['categoryId'];
        this.state.setFilters({ categoryId: params['categoryId'] });
      }
      if (params['regionId']) {
        this.selectedRegionId = params['regionId'];
        this.state.setFilters({ regionId: params['regionId'] });
      }
      if (params['search']) {
        this.searchTerm = params['search'];
        this.state.setFilters({ searchTerm: params['search'] });
      }
    });

    if (!this.selectedCategoryId && !this.selectedRegionId && !this.searchTerm) {
      this.state.loadListings();
    }
  }

  onCategoryChange(categoryId: string): void {
    this.state.clearAttributeFilters();
    this.state.setFilters({ categoryId: categoryId || undefined });
  }

  onRegionChange(regionId: string): void {
    this.state.setFilters({ regionId: regionId || undefined });
  }

  onSearch(): void {
    this.state.setFilters({ searchTerm: this.searchTerm || undefined });
  }

  onSortChange(sortBy: string): void {
    this.state.setFilters({ sortBy: sortBy || undefined });
  }
}
