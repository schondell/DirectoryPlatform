import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { ListingService } from '../../../core/services/listing.service';
import { CategoryService } from '../../../core/services/category.service';
import { Listing, CategoryWithChildren } from '../../../core/models';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslateModule],
  template: `
    <div class="home-container">
      <section class="hero">
        <h1>{{ 'common.home' | translate }}</h1>
        <p>Find what you're looking for</p>
        <div class="search-bar">
          <input type="text" [placeholder]="'common.search' | translate" [(ngModel)]="searchTerm" (keyup.enter)="onSearch()" />
          <button (click)="onSearch()">{{ 'common.search' | translate }}</button>
        </div>
      </section>

      <section class="categories" *ngIf="categories.length">
        <h2>{{ 'common.categories' | translate }}</h2>
        <div class="category-grid">
          @for (cat of categories; track cat.id) {
            <a [routerLink]="['/listings']" [queryParams]="{categoryId: cat.id}" class="category-card">
              <h3>{{ cat.name }}</h3>
              <span class="child-count">{{ cat.children?.length || 0 }} subcategories</span>
            </a>
          }
        </div>
      </section>

      <section class="featured" *ngIf="featuredListings.length">
        <h2>{{ 'listing.featuredListings' | translate }}</h2>
        <div class="listing-grid">
          @for (listing of featuredListings; track listing.id) {
            <a [routerLink]="['/listings', listing.id]" class="listing-card">
              <div class="listing-image" *ngIf="listing.media?.length">
                <img [src]="listing.media[0].url" [alt]="listing.title" />
              </div>
              <div class="listing-info">
                <h3>{{ listing.title }}</h3>
                <p>{{ listing.shortDescription }}</p>
                <span class="category-badge">{{ listing.category?.name }}</span>
                <span class="region-badge" *ngIf="listing.region">{{ listing.region.name }}</span>
              </div>
            </a>
          }
        </div>
      </section>

      <section class="recent" *ngIf="recentListings.length">
        <h2>{{ 'listing.recentListings' | translate }}</h2>
        <div class="listing-grid">
          @for (listing of recentListings; track listing.id) {
            <a [routerLink]="['/listings', listing.id]" class="listing-card">
              <div class="listing-info">
                <h3>{{ listing.title }}</h3>
                <p>{{ listing.shortDescription }}</p>
                <span class="category-badge">{{ listing.category?.name }}</span>
              </div>
            </a>
          }
        </div>
        <a routerLink="/listings" class="view-all">{{ 'common.viewAll' | translate }}</a>
      </section>
    </div>
  `
})
export class HomeComponent implements OnInit {
  categories: CategoryWithChildren[] = [];
  featuredListings: Listing[] = [];
  recentListings: Listing[] = [];
  searchTerm = '';

  constructor(
    private listingService: ListingService,
    private categoryService: CategoryService
  ) {}

  ngOnInit(): void {
    this.categoryService.getTree().subscribe(cats => this.categories = cats);
    this.listingService.getFeatured(6).subscribe(l => this.featuredListings = l);
    this.listingService.getRecent(6).subscribe(l => this.recentListings = l);
  }

  onSearch(): void {
    // Navigate to listings with search term
  }
}
