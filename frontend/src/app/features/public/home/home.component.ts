import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { ListingService } from '../../../core/services/listing.service';
import { CategoryService } from '../../../core/services/category.service';
import { Listing, CategoryWithChildren } from '../../../core/models';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslateModule],
  template: `
    <!-- Hero -->
    <section class="hero">
      <div class="hero__bg"></div>
      <div class="hero__content container">
        <div class="hero__badge animate-in">
          <span class="hero__badge-dot"></span>
          Switzerland's Premium Marketplace
        </div>
        <h1 class="hero__title animate-in animate-in-delay-1">
          Find exactly what<br/>you're looking for
        </h1>
        <p class="hero__subtitle animate-in animate-in-delay-2">
          Browse thousands of listings across 32 categories from verified Swiss sellers
        </p>
        <div class="hero__search animate-in animate-in-delay-3">
          <div class="search-box">
            <svg class="search-box__icon" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="11" cy="11" r="8"/><path d="M21 21l-4.35-4.35"/></svg>
            <input
              type="text"
              class="search-box__input"
              [placeholder]="'common.search' | translate"
              [(ngModel)]="searchTerm"
              (keyup.enter)="onSearch()"
            />
            <select class="search-box__select" [(ngModel)]="selectedCategoryId">
              <option value="">{{ 'common.allCategories' | translate }}</option>
              @for (cat of categories; track cat.id) {
                <option [value]="cat.id">{{ cat.name }}</option>
              }
            </select>
            <button class="search-box__btn" (click)="onSearch()">
              {{ 'common.search' | translate }}
            </button>
          </div>
        </div>
      </div>
    </section>

    <!-- Categories -->
    <section class="section categories-section">
      <div class="container">
        <div class="section-header">
          <h2>{{ 'common.categories' | translate }}</h2>
          <p>Browse by category to find exactly what you need</p>
        </div>
        <div class="cat-grid">
          @for (cat of categories; track cat.id; let i = $index) {
            <a [routerLink]="['/listings']" [queryParams]="{categoryId: cat.id}" class="cat-card animate-in" [style.animation-delay.ms]="40 * i">
              <span class="cat-card__icon">{{ getCategoryIcon(cat.slug) }}</span>
              <span class="cat-card__name">{{ cat.name }}</span>
              @if (cat.children?.length) {
                <span class="cat-card__count">{{ cat.children.length }}</span>
              }
            </a>
          }
        </div>
      </div>
    </section>

    <!-- Featured Listings -->
    @if (featuredListings.length) {
      <section class="section section--sm featured-section">
        <div class="container">
          <div class="section-header">
            <h2>{{ 'listing.featuredListings' | translate }}</h2>
            <a routerLink="/listings" [queryParams]="{sortBy: 'featured'}" class="section-header__link">
              {{ 'common.viewAll' | translate }}
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M5 12h14M12 5l7 7-7 7"/></svg>
            </a>
          </div>
          <div class="listing-grid">
            @for (listing of featuredListings; track listing.id; let i = $index) {
              <a [routerLink]="['/listings', listing.id]" class="listing-card card card--hover animate-in" [style.animation-delay.ms]="60 * i">
                <div class="listing-card__image">
                  @if (listing.media?.length) {
                    <img [src]="listing.media[0].url" [alt]="listing.title" loading="lazy" />
                  } @else {
                    <div class="listing-card__placeholder">
                      <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" opacity="0.3">
                        <rect x="3" y="3" width="18" height="18" rx="2"/><circle cx="8.5" cy="8.5" r="1.5"/><path d="M21 15l-5-5L5 21"/>
                      </svg>
                    </div>
                  }
                  @if (listing.isFeatured) {
                    <span class="listing-card__badge badge badge--accent">Featured</span>
                  }
                  @if (listing.isPremium) {
                    <span class="listing-card__badge listing-card__badge--top badge badge--dark">Premium</span>
                  }
                </div>
                <div class="listing-card__body">
                  <div class="listing-card__meta">
                    @if (listing.category) {
                      <span class="badge badge--neutral">{{ listing.category.name }}</span>
                    }
                    @if (listing.region) {
                      <span class="listing-card__region">{{ listing.region.name }}</span>
                    }
                  </div>
                  <h3 class="listing-card__title">{{ listing.title }}</h3>
                  @if (listing.shortDescription) {
                    <p class="listing-card__desc">{{ listing.shortDescription }}</p>
                  }
                  <div class="listing-card__footer">
                    @if (listing.detail?.priceInfo) {
                      <span class="listing-card__price">{{ listing.detail?.priceInfo }}</span>
                    }
                    <div class="listing-card__stats">
                      <span class="listing-card__stat">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/></svg>
                        {{ listing.viewCount }}
                      </span>
                    </div>
                  </div>
                </div>
              </a>
            }
          </div>
        </div>
      </section>
    }

    <!-- Recent Listings -->
    @if (recentListings.length) {
      <section class="section section--sm recent-section">
        <div class="container">
          <div class="section-header">
            <h2>{{ 'listing.recentListings' | translate }}</h2>
            <a routerLink="/listings" class="section-header__link">
              {{ 'common.viewAll' | translate }}
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M5 12h14M12 5l7 7-7 7"/></svg>
            </a>
          </div>
          <div class="recent-list">
            @for (listing of recentListings; track listing.id; let i = $index) {
              <a [routerLink]="['/listings', listing.id]" class="recent-item animate-in" [style.animation-delay.ms]="40 * i">
                <div class="recent-item__thumb">
                  @if (listing.media?.length) {
                    <img [src]="listing.media[0].url" [alt]="listing.title" loading="lazy" />
                  } @else {
                    <div class="recent-item__placeholder">
                      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" opacity="0.3">
                        <rect x="3" y="3" width="18" height="18" rx="2"/><circle cx="8.5" cy="8.5" r="1.5"/><path d="M21 15l-5-5L5 21"/>
                      </svg>
                    </div>
                  }
                </div>
                <div class="recent-item__info">
                  <h4 class="recent-item__title">{{ listing.title }}</h4>
                  <div class="recent-item__meta">
                    @if (listing.category) {
                      <span>{{ listing.category.name }}</span>
                    }
                    @if (listing.region) {
                      <span>{{ listing.region.name }}</span>
                    }
                  </div>
                </div>
                <div class="recent-item__right">
                  @if (listing.detail?.priceInfo) {
                    <span class="recent-item__price">{{ listing.detail?.priceInfo }}</span>
                  }
                  <span class="recent-item__views">
                    <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/></svg>
                    {{ listing.viewCount }}
                  </span>
                </div>
              </a>
            }
          </div>
        </div>
      </section>
    }

    <!-- Stats -->
    <section class="section stats-section">
      <div class="container">
        <div class="stats-row">
          <div class="stat-block animate-in">
            <div class="stat-block__value">32</div>
            <div class="stat-block__label">Categories</div>
          </div>
          <div class="stat-block animate-in animate-in-delay-1">
            <div class="stat-block__value">130+</div>
            <div class="stat-block__label">Subcategories</div>
          </div>
          <div class="stat-block animate-in animate-in-delay-2">
            <div class="stat-block__value">26</div>
            <div class="stat-block__label">Swiss Cantons</div>
          </div>
          <div class="stat-block animate-in animate-in-delay-3">
            <div class="stat-block__value">CHF</div>
            <div class="stat-block__label">Swiss Currency</div>
          </div>
        </div>
      </div>
    </section>

    <!-- CTA -->
    <section class="section cta-section">
      <div class="container">
        <div class="cta-card animate-in">
          <div class="cta-card__content">
            <h2 class="cta-card__title">Ready to sell?</h2>
            <p class="cta-card__text">Create your listing in minutes and reach thousands of potential buyers across Switzerland.</p>
            <div class="cta-card__actions">
              <a routerLink="/auth/register" class="btn btn--primary btn--lg">Get Started Free</a>
              <a routerLink="/pricing" class="btn btn--outline btn--lg">View Plans</a>
            </div>
          </div>
          <div class="cta-card__decoration">
            <div class="cta-orb cta-orb--1"></div>
            <div class="cta-orb cta-orb--2"></div>
          </div>
        </div>
      </div>
    </section>
  `,
  styles: [`
    // ─── Hero ───────────────────────────────────────────────
    .hero {
      position: relative;
      padding: var(--space-20) 0 var(--space-16);
      overflow: hidden;
    }

    .hero__bg {
      position: absolute;
      inset: 0;
      background: linear-gradient(135deg, #0f172a 0%, #1e293b 50%, #334155 100%);
      &::after {
        content: '';
        position: absolute;
        inset: 0;
        background: radial-gradient(ellipse at 70% 20%, rgba(232, 89, 12, 0.15) 0%, transparent 60%),
                    radial-gradient(ellipse at 20% 80%, rgba(37, 99, 235, 0.08) 0%, transparent 50%);
      }
    }

    .hero__content {
      position: relative;
      text-align: center;
      max-width: 720px;
      margin: 0 auto;
    }

    .hero__badge {
      display: inline-flex;
      align-items: center;
      gap: var(--space-2);
      font-family: var(--font-display);
      font-size: var(--text-xs);
      font-weight: 600;
      letter-spacing: 0.04em;
      color: rgba(255,255,255,0.7);
      background: rgba(255,255,255,0.08);
      border: 1px solid rgba(255,255,255,0.1);
      padding: var(--space-2) var(--space-4);
      border-radius: var(--radius-full);
      margin-bottom: var(--space-6);
    }

    .hero__badge-dot {
      width: 6px;
      height: 6px;
      border-radius: 50%;
      background: var(--color-accent);
      animation: pulse 2s ease-in-out infinite;
    }

    .hero__title {
      font-size: var(--text-5xl);
      font-weight: 800;
      color: white;
      letter-spacing: -0.03em;
      margin-bottom: var(--space-5);
      line-height: 1.1;

      @media (max-width: 768px) { font-size: var(--text-3xl); }
    }

    .hero__subtitle {
      font-size: var(--text-lg);
      color: rgba(255,255,255,0.55);
      margin-bottom: var(--space-10);
      max-width: 520px;
      margin-left: auto;
      margin-right: auto;
    }

    // ─── Search Box ──────────────────────────────────────────

    .hero__search { max-width: 640px; margin: 0 auto; }

    .search-box {
      display: flex;
      align-items: center;
      background: white;
      border-radius: var(--radius-lg);
      box-shadow: 0 20px 60px rgba(0,0,0,0.3), 0 0 0 1px rgba(255,255,255,0.1);
      overflow: hidden;
      height: 56px;

      @media (max-width: 640px) {
        flex-wrap: wrap;
        height: auto;
        border-radius: var(--radius-md);
      }

      &__icon {
        margin-left: var(--space-5);
        color: var(--color-ink-faint);
        flex-shrink: 0;
        @media (max-width: 640px) { margin-left: var(--space-4); }
      }

      &__input {
        flex: 1;
        border: none;
        outline: none;
        padding: 0 var(--space-4);
        font-size: var(--text-base);
        font-family: var(--font-body);
        color: var(--color-ink);
        background: transparent;
        min-width: 0;

        &::placeholder { color: var(--color-ink-faint); }

        @media (max-width: 640px) {
          padding: var(--space-3) var(--space-3);
        }
      }

      &__select {
        appearance: none;
        border: none;
        border-left: 1px solid var(--color-border);
        padding: 0 var(--space-5) 0 var(--space-4);
        font-family: var(--font-display);
        font-size: var(--text-sm);
        font-weight: 500;
        color: var(--color-ink-light);
        background: transparent;
        outline: none;
        cursor: pointer;
        max-width: 160px;

        @media (max-width: 640px) {
          border-left: none;
          border-top: 1px solid var(--color-border);
          max-width: 100%;
          width: 100%;
          padding: var(--space-3) var(--space-4);
        }
      }

      &__btn {
        height: 100%;
        padding: 0 var(--space-6);
        background: var(--color-accent);
        color: white;
        border: none;
        font-family: var(--font-display);
        font-weight: 600;
        font-size: var(--text-sm);
        cursor: pointer;
        transition: background var(--duration-fast) var(--ease-out);
        white-space: nowrap;
        flex-shrink: 0;

        &:hover { background: var(--color-accent-dark); }

        @media (max-width: 640px) {
          width: 100%;
          height: 48px;
          border-radius: 0 0 var(--radius-md) var(--radius-md);
        }
      }
    }

    // ─── Section Header ─────────────────────────────────────

    .section-header {
      display: flex;
      align-items: baseline;
      justify-content: space-between;
      margin-bottom: var(--space-8);
      gap: var(--space-4);

      h2 {
        font-size: var(--text-2xl);
        font-weight: 700;
      }

      p {
        font-size: var(--text-sm);
        color: var(--color-ink-muted);
        margin-top: var(--space-1);
      }

      &__link {
        display: inline-flex;
        align-items: center;
        gap: var(--space-1);
        font-family: var(--font-display);
        font-size: var(--text-sm);
        font-weight: 600;
        color: var(--color-accent);
        white-space: nowrap;
        transition: gap var(--duration-fast) var(--ease-out);
        &:hover { gap: var(--space-2); }
      }

      @media (max-width: 640px) {
        flex-direction: column;
      }
    }

    // ─── Category Grid ──────────────────────────────────────

    .cat-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
      gap: var(--space-3);

      @media (max-width: 640px) {
        grid-template-columns: repeat(2, 1fr);
      }
    }

    .cat-card {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: var(--space-2);
      padding: var(--space-5) var(--space-3);
      background: var(--color-surface-raised);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-lg);
      text-align: center;
      transition: all var(--duration-normal) var(--ease-out);

      &:hover {
        border-color: var(--color-accent);
        box-shadow: var(--shadow-md);
        transform: translateY(-2px);
      }

      &__icon {
        font-size: 1.75rem;
        line-height: 1;
      }

      &__name {
        font-family: var(--font-display);
        font-size: var(--text-xs);
        font-weight: 600;
        color: var(--color-ink);
        line-height: var(--leading-snug);
      }

      &__count {
        font-size: 0.625rem;
        color: var(--color-ink-faint);
        background: var(--color-surface-sunken);
        padding: 1px 6px;
        border-radius: var(--radius-full);
      }
    }

    // ─── Listing Grid ────────────────────────────────────────

    .listing-grid {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: var(--space-6);

      @media (max-width: 960px) { grid-template-columns: repeat(2, 1fr); }
      @media (max-width: 640px) { grid-template-columns: 1fr; }
    }

    .listing-card {
      display: flex;
      flex-direction: column;
      overflow: hidden;
      text-decoration: none;

      &__image {
        position: relative;
        aspect-ratio: 16/10;
        overflow: hidden;
        background: var(--color-surface-sunken);

        img {
          width: 100%;
          height: 100%;
          object-fit: cover;
          transition: transform var(--duration-slow) var(--ease-out);
        }
      }

      &:hover .listing-card__image img {
        transform: scale(1.04);
      }

      &__placeholder {
        width: 100%;
        height: 100%;
        display: flex;
        align-items: center;
        justify-content: center;
        background: var(--color-surface-sunken);
      }

      &__badge {
        position: absolute;
        bottom: var(--space-3);
        left: var(--space-3);

        &--top {
          bottom: auto;
          top: var(--space-3);
          left: auto;
          right: var(--space-3);
        }
      }

      &__body {
        padding: var(--space-4) var(--space-5) var(--space-5);
        display: flex;
        flex-direction: column;
        gap: var(--space-2);
        flex: 1;
      }

      &__meta {
        display: flex;
        align-items: center;
        gap: var(--space-2);
      }

      &__region {
        font-size: var(--text-xs);
        color: var(--color-ink-muted);
      }

      &__title {
        font-size: var(--text-base);
        font-weight: 600;
        color: var(--color-ink);
        line-height: var(--leading-snug);
        display: -webkit-box;
        -webkit-line-clamp: 2;
        -webkit-box-orient: vertical;
        overflow: hidden;
      }

      &__desc {
        font-size: var(--text-sm);
        color: var(--color-ink-muted);
        display: -webkit-box;
        -webkit-line-clamp: 2;
        -webkit-box-orient: vertical;
        overflow: hidden;
      }

      &__footer {
        display: flex;
        align-items: center;
        justify-content: space-between;
        margin-top: auto;
        padding-top: var(--space-3);
        border-top: 1px solid var(--color-border);
      }

      &__price {
        font-family: var(--font-display);
        font-weight: 700;
        font-size: var(--text-base);
        color: var(--color-accent-dark);
      }

      &__stats {
        display: flex;
        gap: var(--space-3);
      }

      &__stat {
        display: inline-flex;
        align-items: center;
        gap: 4px;
        font-size: var(--text-xs);
        color: var(--color-ink-faint);
      }
    }

    // ─── Recent List ─────────────────────────────────────────

    .recent-list {
      display: flex;
      flex-direction: column;
      gap: var(--space-2);
    }

    .recent-item {
      display: flex;
      align-items: center;
      gap: var(--space-4);
      padding: var(--space-3) var(--space-4);
      background: var(--color-surface-raised);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      transition: all var(--duration-fast) var(--ease-out);

      &:hover {
        border-color: var(--color-border-strong);
        box-shadow: var(--shadow-sm);
      }

      &__thumb {
        width: 56px;
        height: 56px;
        border-radius: var(--radius-sm);
        overflow: hidden;
        flex-shrink: 0;
        background: var(--color-surface-sunken);

        img { width: 100%; height: 100%; object-fit: cover; }
      }

      &__placeholder {
        width: 100%;
        height: 100%;
        display: flex;
        align-items: center;
        justify-content: center;
      }

      &__info { flex: 1; min-width: 0; }

      &__title {
        font-size: var(--text-sm);
        font-weight: 600;
        color: var(--color-ink);
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
      }

      &__meta {
        display: flex;
        gap: var(--space-2);
        font-size: var(--text-xs);
        color: var(--color-ink-muted);
        margin-top: 2px;

        span + span::before {
          content: '·';
          margin-right: var(--space-2);
        }
      }

      &__right {
        display: flex;
        flex-direction: column;
        align-items: flex-end;
        gap: var(--space-1);
        flex-shrink: 0;
      }

      &__price {
        font-family: var(--font-display);
        font-weight: 700;
        font-size: var(--text-sm);
        color: var(--color-accent-dark);
      }

      &__views {
        display: inline-flex;
        align-items: center;
        gap: 3px;
        font-size: var(--text-xs);
        color: var(--color-ink-faint);
      }
    }

    // ─── Stats Section ────────────────────────────────────────

    .stats-section {
      background: var(--color-surface-sunken);
    }

    .stats-row {
      display: grid;
      grid-template-columns: repeat(4, 1fr);
      gap: var(--space-6);
      text-align: center;

      @media (max-width: 640px) {
        grid-template-columns: repeat(2, 1fr);
      }
    }

    .stat-block {
      &__value {
        font-family: var(--font-display);
        font-size: var(--text-4xl);
        font-weight: 800;
        color: var(--color-ink);
        letter-spacing: -0.02em;
      }

      &__label {
        font-family: var(--font-display);
        font-size: var(--text-xs);
        font-weight: 600;
        text-transform: uppercase;
        letter-spacing: 0.06em;
        color: var(--color-ink-muted);
        margin-top: var(--space-1);
      }
    }

    // ─── CTA Section ──────────────────────────────────────────

    .cta-section {
      padding-bottom: var(--space-20);
    }

    .cta-card {
      position: relative;
      background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
      border-radius: var(--radius-xl);
      padding: var(--space-16) var(--space-12);
      overflow: hidden;
      text-align: center;

      @media (max-width: 640px) {
        padding: var(--space-10) var(--space-6);
      }

      &__content { position: relative; z-index: 1; }

      &__title {
        font-size: var(--text-3xl);
        font-weight: 800;
        color: white;
        margin-bottom: var(--space-4);
      }

      &__text {
        font-size: var(--text-base);
        color: rgba(255,255,255,0.55);
        max-width: 480px;
        margin: 0 auto var(--space-8);
      }

      &__actions {
        display: flex;
        justify-content: center;
        gap: var(--space-4);
        flex-wrap: wrap;

        .btn--outline {
          border-color: rgba(255,255,255,0.25);
          color: white;
          &:hover {
            border-color: white;
            background: rgba(255,255,255,0.08);
          }
        }
      }

      &__decoration {
        position: absolute;
        inset: 0;
        pointer-events: none;
      }
    }

    .cta-orb {
      position: absolute;
      border-radius: 50%;
      filter: blur(80px);

      &--1 {
        width: 300px;
        height: 300px;
        background: rgba(232, 89, 12, 0.2);
        top: -80px;
        right: -60px;
      }

      &--2 {
        width: 200px;
        height: 200px;
        background: rgba(37, 99, 235, 0.12);
        bottom: -60px;
        left: -40px;
      }
    }
  `]
})
export class HomeComponent implements OnInit {
  categories: CategoryWithChildren[] = [];
  featuredListings: Listing[] = [];
  recentListings: Listing[] = [];
  searchTerm = '';
  selectedCategoryId = '';

  private readonly categoryIcons: Record<string, string> = {
    'automobiles': '🚗', 'motos': '🏍️', 'vehicules-utilitaires': '🚛', 'bateaux': '⛵',
    'immobilier': '🏠', 'informatique': '💻', 'telephonie': '📱', 'image-son': '📺',
    'electromenager': '🏪', 'ameublement': '🛋️', 'jardinage-bricolage': '🔧', 'sport-loisirs': '⚽',
    'vetements-accessoires': '👔', 'beaute-bien-etre': '💄', 'livres-musique-films': '📚',
    'jeux-jouets': '🎮', 'animaux': '🐾', 'emploi': '💼', 'services': '🤝',
    'cours-formations': '🎓', 'billets-evenements': '🎫', 'collections-art': '🎨',
    'montres-bijoux': '⌚', 'instruments-musique': '🎵', 'materiel-professionnel': '🏗️',
    'armes': '🎯', 'vins-gastronomie': '🍷', 'enfants-bebe': '👶', 'sante': '❤️',
    'voyages-vacances': '✈️', 'agriculture': '🌾', 'divers': '📦'
  };

  constructor(
    private listingService: ListingService,
    private categoryService: CategoryService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.categoryService.getTree().subscribe(cats => this.categories = cats);
    this.listingService.getFeatured(6).subscribe(l => this.featuredListings = l);
    this.listingService.getRecent(6).subscribe(l => this.recentListings = l);
  }

  onSearch(): void {
    const params: Record<string, string> = {};
    if (this.searchTerm) params['search'] = this.searchTerm;
    if (this.selectedCategoryId) params['categoryId'] = this.selectedCategoryId;
    this.router.navigate(['/listings'], { queryParams: params });
  }

  getCategoryIcon(slug: string): string {
    return this.categoryIcons[slug] || '📂';
  }
}
