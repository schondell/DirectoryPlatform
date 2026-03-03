import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { CategoryService } from '../../../core/services/category.service';
import { RegionService } from '../../../core/services/region.service';
import { ListingService } from '../../../core/services/listing.service';
import { AiListingService } from '../../../core/services/ai-listing.service';
import {
  Category, CategoryWithChildren, Region, RegionWithChildren,
  AttributeDefinition, AiGeneratedListing, CreateListingRequest
} from '../../../core/models';

@Component({
  selector: 'app-create-listing',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslateModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="create-listing">
      <div class="container">
        <div class="page-header">
          <a routerLink="/member/listings" class="back-link">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M19 12H5M12 19l-7-7 7-7"/></svg>
            Back to listings
          </a>
          <h1 class="page-header__title">Create New Listing</h1>
          <p class="page-header__sub">Fill in the details or let AI generate a professional listing for you</p>
        </div>

        <div class="two-panel">
          <!-- LEFT PANEL — Form -->
          <div class="panel panel--form">
            <div class="card">
              <div class="card__header"><h2>Category</h2></div>
              <div class="card__body">
                <div class="field">
                  <label class="field__label">Parent Category</label>
                  <select class="field__input" [(ngModel)]="selectedParentId" (ngModelChange)="onParentCategoryChange($event)">
                    <option value="">-- Select category --</option>
                    @for (cat of parentCategories; track cat.id) {
                      <option [value]="cat.id">{{ cat.name }}</option>
                    }
                  </select>
                </div>
                @if (childCategories.length) {
                  <div class="field">
                    <label class="field__label">Subcategory</label>
                    <select class="field__input" [(ngModel)]="form.categoryId" (ngModelChange)="onCategoryChange($event)">
                      <option value="">-- Select subcategory --</option>
                      @for (cat of childCategories; track cat.id) {
                        <option [value]="cat.id">{{ cat.name }}</option>
                      }
                    </select>
                  </div>
                }
              </div>
            </div>

            <div class="card">
              <div class="card__header"><h2>Listing Details</h2></div>
              <div class="card__body">
                <div class="field">
                  <label class="field__label">Title *</label>
                  <input class="field__input" type="text" [(ngModel)]="form.title" placeholder="Title of your listing" maxlength="200">
                </div>
                <div class="field">
                  <label class="field__label">Short Description</label>
                  <input class="field__input" type="text" [(ngModel)]="form.shortDescription" placeholder="Brief summary" maxlength="300">
                </div>
                <div class="field">
                  <label class="field__label">Description</label>
                  <textarea class="field__input field__textarea" [(ngModel)]="form.description" placeholder="Detailed description of your item or service" rows="6"></textarea>
                </div>
                <div class="field">
                  <label class="field__label">Price Info</label>
                  <input class="field__input" type="text" [(ngModel)]="form.priceInfo" placeholder="e.g. CHF 150, Price on request, Free">
                </div>
              </div>
            </div>

            @if (attributeDefinitions.length) {
              <div class="card">
                <div class="card__header"><h2>Category Attributes</h2></div>
                <div class="card__body">
                  @for (attr of attributeDefinitions; track attr.id) {
                    <div class="field">
                      <label class="field__label">
                        {{ attr.name }}
                        @if (attr.isRequired) { <span class="field__required">*</span> }
                        @if (attr.unit) { <span class="field__unit">({{ attr.unit }})</span> }
                      </label>
                      @if (attr.description) {
                        <span class="field__hint">{{ attr.description }}</span>
                      }

                      @switch (attr.type) {
                        @case ('Text') {
                          <input class="field__input" type="text" [ngModel]="getAttributeValue(attr.id)" (ngModelChange)="setAttributeValue(attr.id, $event)">
                        }
                        @case ('Number') {
                          <div class="field__with-unit">
                            <input class="field__input" type="number" [min]="attr.minValue ?? ''" [max]="attr.maxValue ?? ''" [ngModel]="getAttributeValue(attr.id)" (ngModelChange)="setAttributeValue(attr.id, $event)">
                            @if (attr.unit) { <span class="field__unit-label">{{ attr.unit }}</span> }
                          </div>
                        }
                        @case ('Boolean') {
                          <label class="field__checkbox">
                            <input type="checkbox" [ngModel]="getAttributeValue(attr.id) === 'true'" (ngModelChange)="setAttributeValue(attr.id, $event ? 'true' : 'false')">
                            <span>{{ attr.name }}</span>
                          </label>
                        }
                        @case ('Select') {
                          <select class="field__input" [ngModel]="getAttributeValue(attr.id)" (ngModelChange)="setAttributeValue(attr.id, $event)">
                            <option value="">-- Select --</option>
                            @for (opt of attr.options ?? []; track opt) {
                              <option [value]="opt">{{ opt }}</option>
                            }
                          </select>
                        }
                        @case ('MultiSelect') {
                          <div class="field__multi">
                            @for (opt of attr.options ?? []; track opt) {
                              <label class="field__checkbox">
                                <input type="checkbox" [checked]="isMultiSelected(attr.id, opt)" (change)="toggleMultiSelect(attr.id, opt)">
                                <span>{{ opt }}</span>
                              </label>
                            }
                          </div>
                        }
                        @case ('Date') {
                          <input class="field__input" type="date" [ngModel]="getAttributeValue(attr.id)" (ngModelChange)="setAttributeValue(attr.id, $event)">
                        }
                      }
                    </div>
                  }
                </div>
              </div>
            }

            <div class="card">
              <div class="card__header"><h2>Location & Contact</h2></div>
              <div class="card__body">
                <div class="field-row">
                  <div class="field">
                    <label class="field__label">Region</label>
                    <select class="field__input" [(ngModel)]="form.regionId">
                      <option value="">-- Select region --</option>
                      @for (r of regions; track r.id) {
                        <option [value]="r.id">{{ r.name }}</option>
                      }
                    </select>
                  </div>
                  <div class="field">
                    <label class="field__label">Town / City</label>
                    <input class="field__input" type="text" [(ngModel)]="form.town" placeholder="e.g. Lausanne">
                  </div>
                </div>
                <div class="field-row">
                  <div class="field">
                    <label class="field__label">Phone</label>
                    <input class="field__input" type="tel" [(ngModel)]="form.phone" placeholder="+41 xx xxx xx xx">
                  </div>
                  <div class="field">
                    <label class="field__label">Email</label>
                    <input class="field__input" type="email" [(ngModel)]="form.email" placeholder="contact@example.com">
                  </div>
                </div>
                <div class="field-row">
                  <div class="field">
                    <label class="field__label">Website</label>
                    <input class="field__input" type="url" [(ngModel)]="form.website" placeholder="https://...">
                  </div>
                  <div class="field">
                    <label class="field__label">Address</label>
                    <input class="field__input" type="text" [(ngModel)]="form.address" placeholder="Street, zip, city">
                  </div>
                </div>
              </div>
            </div>

            <div class="form-actions">
              <button class="btn btn--primary btn--lg" (click)="submitListing()" [disabled]="submitting">
                @if (submitting) {
                  <span class="spinner"></span> Publishing...
                } @else {
                  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><path d="M22 2L11 13M22 2l-7 20-4-9-9-4 20-7z"/></svg>
                  Publish Listing
                }
              </button>
              <a routerLink="/member/listings" class="btn btn--outline btn--lg">Cancel</a>
            </div>
          </div>

          <!-- RIGHT PANEL — AI Assistant -->
          <div class="panel panel--ai">
            <div class="ai-panel">
              <div class="ai-panel__header">
                <div class="ai-panel__icon">
                  <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 2a4 4 0 014 4v1a4 4 0 01-8 0V6a4 4 0 014-4z"/><path d="M6.8 22a7.2 7.2 0 0110.4 0"/><circle cx="12" cy="14" r="4"/></svg>
                </div>
                <div>
                  <h3 class="ai-panel__title">AI Assistant</h3>
                  <p class="ai-panel__sub">Generate professional listings instantly</p>
                </div>
              </div>

              @if (!form.categoryId) {
                <div class="ai-panel__notice">
                  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><path d="M12 16v-4M12 8h.01"/></svg>
                  Select a category first to enable AI generation
                </div>
              }

              <div class="ai-panel__body">
                <div class="field">
                  <label class="field__label">Describe your item in 1-2 sentences</label>
                  <textarea class="field__input field__textarea" [(ngModel)]="aiInput" placeholder="e.g. Selling a used iPhone 14 Pro 256GB in excellent condition, bought in January 2025" rows="3" [disabled]="!form.categoryId"></textarea>
                </div>

                <div class="field">
                  <label class="field__label">Language</label>
                  <select class="field__input" [(ngModel)]="aiLanguage">
                    <option value="fr">Francais</option>
                    <option value="de">Deutsch</option>
                    <option value="it">Italiano</option>
                    <option value="en">English</option>
                  </select>
                </div>

                <div class="ai-panel__actions">
                  <button class="btn btn--accent" (click)="generateWithAi()" [disabled]="!form.categoryId || !aiInput.trim() || aiLoading">
                    @if (aiLoading) {
                      <span class="spinner spinner--sm"></span> Generating...
                    } @else {
                      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><path d="M13 2L3 14h9l-1 8 10-12h-9l1-8z"/></svg>
                      Generate with AI
                    }
                  </button>
                  @if (form.title || form.description) {
                    <button class="btn btn--outline" (click)="improveWithAi()" [disabled]="!form.categoryId || aiLoading">
                      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 20h9M16.5 3.5a2.121 2.121 0 013 3L7 19l-4 1 1-4L16.5 3.5z"/></svg>
                      Improve
                    </button>
                  }
                </div>

                @if (aiError) {
                  <div class="ai-panel__error">{{ aiError }}</div>
                }

                @if (aiResult) {
                  <div class="ai-result">
                    <div class="ai-result__header">
                      <h4>AI Generated Result</h4>
                      <button class="btn btn--primary btn--sm" (click)="applyAiResult()">
                        <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><path d="M20 6L9 17l-5-5"/></svg>
                        Apply All
                      </button>
                    </div>

                    <div class="ai-result__field">
                      <span class="ai-result__label">Title</span>
                      <span class="ai-result__value">{{ aiResult.title }}</span>
                    </div>

                    <div class="ai-result__field">
                      <span class="ai-result__label">Short Description</span>
                      <span class="ai-result__value">{{ aiResult.shortDescription }}</span>
                    </div>

                    <div class="ai-result__field">
                      <span class="ai-result__label">Description</span>
                      <p class="ai-result__value ai-result__value--desc">{{ aiResult.description }}</p>
                    </div>

                    @if (aiResult.suggestedPriceMin || aiResult.suggestedPriceMax) {
                      <div class="ai-result__field">
                        <span class="ai-result__label">Suggested Price</span>
                        <span class="ai-result__value ai-result__value--price">
                          CHF {{ aiResult.suggestedPriceMin | number:'1.0-0' }} - {{ aiResult.suggestedPriceMax | number:'1.0-0' }}
                        </span>
                      </div>
                    }

                    @if (aiResult.attributes.length) {
                      <div class="ai-result__field">
                        <span class="ai-result__label">Suggested Attributes</span>
                        <div class="ai-result__attrs">
                          @for (attr of aiResult.attributes; track attr.attributeDefinitionId) {
                            <div class="ai-result__attr">
                              <span class="ai-result__attr-name">{{ attr.name }}</span>
                              <span class="ai-result__attr-value">{{ attr.value }}</span>
                            </div>
                          }
                        </div>
                      </div>
                    }
                  </div>
                }
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .create-listing {
      padding: var(--space-8) 0 var(--space-16);
    }

    .page-header {
      margin-bottom: var(--space-8);

      &__title {
        font-size: var(--text-2xl);
        font-weight: 700;
        margin-top: var(--space-3);
      }

      &__sub {
        font-size: var(--text-sm);
        color: var(--color-ink-muted);
        margin-top: var(--space-1);
      }
    }

    .back-link {
      display: inline-flex;
      align-items: center;
      gap: var(--space-2);
      font-size: var(--text-sm);
      color: var(--color-ink-muted);
      transition: color var(--duration-fast) var(--ease-out);
      &:hover { color: var(--color-accent); }
    }

    // ─── Two Panel Layout ──────────────────────────────────
    .two-panel {
      display: grid;
      grid-template-columns: 1fr 400px;
      gap: var(--space-6);
      align-items: start;

      @media (max-width: 1024px) {
        grid-template-columns: 1fr;
      }
    }

    .panel--ai {
      position: sticky;
      top: var(--space-4);

      @media (max-width: 1024px) {
        position: static;
        order: -1;
      }
    }

    // ─── Cards ─────────────────────────────────────────────
    .card {
      background: var(--color-surface-raised);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-lg);
      margin-bottom: var(--space-4);

      &__header {
        padding: var(--space-4) var(--space-5);
        border-bottom: 1px solid var(--color-border);

        h2 {
          font-size: var(--text-base);
          font-weight: 600;
        }
      }

      &__body {
        padding: var(--space-5);
        display: flex;
        flex-direction: column;
        gap: var(--space-4);
      }
    }

    // ─── Form Fields ───────────────────────────────────────
    .field {
      display: flex;
      flex-direction: column;
      gap: var(--space-1);
      flex: 1;

      &__label {
        font-size: var(--text-sm);
        font-weight: 600;
        color: var(--color-ink);
      }

      &__required { color: var(--color-danger); }

      &__unit {
        font-weight: 400;
        color: var(--color-ink-muted);
        font-size: var(--text-xs);
      }

      &__hint {
        font-size: var(--text-xs);
        color: var(--color-ink-faint);
      }

      &__input {
        padding: var(--space-2) var(--space-3);
        border: 1px solid var(--color-border);
        border-radius: var(--radius-md);
        font-size: var(--text-sm);
        color: var(--color-ink);
        background: var(--color-surface);
        transition: border-color var(--duration-fast) var(--ease-out), box-shadow var(--duration-fast) var(--ease-out);

        &:focus {
          outline: none;
          border-color: var(--color-accent);
          box-shadow: 0 0 0 3px var(--color-accent-bg);
        }

        &:disabled {
          opacity: 0.5;
          cursor: not-allowed;
        }
      }

      &__textarea {
        resize: vertical;
        min-height: 80px;
        font-family: inherit;
      }

      &__with-unit {
        display: flex;
        align-items: center;
        gap: var(--space-2);

        .field__input { flex: 1; }
      }

      &__unit-label {
        font-size: var(--text-sm);
        color: var(--color-ink-muted);
        white-space: nowrap;
      }

      &__checkbox {
        display: flex;
        align-items: center;
        gap: var(--space-2);
        font-size: var(--text-sm);
        cursor: pointer;

        input[type="checkbox"] {
          width: 16px;
          height: 16px;
          accent-color: var(--color-accent);
        }
      }

      &__multi {
        display: flex;
        flex-wrap: wrap;
        gap: var(--space-3);
      }
    }

    .field-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--space-4);

      @media (max-width: 640px) {
        grid-template-columns: 1fr;
      }
    }

    // ─── Form Actions ──────────────────────────────────────
    .form-actions {
      display: flex;
      gap: var(--space-3);
      margin-top: var(--space-4);
    }

    // ─── AI Panel ──────────────────────────────────────────
    .ai-panel {
      background: var(--color-surface-raised);
      border: 1px solid var(--color-accent);
      border-radius: var(--radius-lg);
      overflow: hidden;

      &__header {
        display: flex;
        align-items: center;
        gap: var(--space-3);
        padding: var(--space-4) var(--space-5);
        background: var(--color-accent-bg);
        border-bottom: 1px solid var(--color-accent);
      }

      &__icon {
        width: 40px;
        height: 40px;
        border-radius: var(--radius-md);
        background: var(--color-accent);
        color: white;
        display: flex;
        align-items: center;
        justify-content: center;
        flex-shrink: 0;
      }

      &__title {
        font-size: var(--text-base);
        font-weight: 700;
        color: var(--color-ink);
      }

      &__sub {
        font-size: var(--text-xs);
        color: var(--color-ink-muted);
      }

      &__body {
        padding: var(--space-5);
        display: flex;
        flex-direction: column;
        gap: var(--space-4);
      }

      &__actions {
        display: flex;
        gap: var(--space-3);
        flex-wrap: wrap;
      }

      &__notice {
        display: flex;
        align-items: center;
        gap: var(--space-2);
        padding: var(--space-3) var(--space-5);
        font-size: var(--text-xs);
        color: var(--color-ink-muted);
        background: var(--color-surface-sunken);
        border-bottom: 1px solid var(--color-border);
      }

      &__error {
        padding: var(--space-3);
        background: var(--color-danger-bg);
        color: var(--color-danger);
        border-radius: var(--radius-md);
        font-size: var(--text-sm);
      }
    }

    // ─── AI Result ─────────────────────────────────────────
    .ai-result {
      background: var(--color-surface-sunken);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      padding: var(--space-4);

      &__header {
        display: flex;
        align-items: center;
        justify-content: space-between;
        margin-bottom: var(--space-4);

        h4 {
          font-size: var(--text-sm);
          font-weight: 600;
        }
      }

      &__field {
        margin-bottom: var(--space-3);

        &:last-child { margin-bottom: 0; }
      }

      &__label {
        display: block;
        font-size: var(--text-xs);
        font-weight: 600;
        color: var(--color-ink-muted);
        margin-bottom: var(--space-1);
        text-transform: uppercase;
        letter-spacing: 0.05em;
      }

      &__value {
        font-size: var(--text-sm);
        color: var(--color-ink);
        line-height: 1.5;

        &--desc {
          white-space: pre-line;
          margin: 0;
        }

        &--price {
          font-weight: 700;
          color: var(--color-accent-dark);
        }
      }

      &__attrs {
        display: flex;
        flex-wrap: wrap;
        gap: var(--space-2);
      }

      &__attr {
        display: inline-flex;
        align-items: center;
        gap: var(--space-1);
        padding: var(--space-1) var(--space-2);
        background: var(--color-surface-raised);
        border: 1px solid var(--color-border);
        border-radius: var(--radius-sm);
        font-size: var(--text-xs);
      }

      &__attr-name {
        font-weight: 600;
        color: var(--color-ink-muted);
      }

      &__attr-value {
        color: var(--color-ink);
      }
    }

    // ─── Buttons ───────────────────────────────────────────
    .btn {
      display: inline-flex;
      align-items: center;
      gap: var(--space-2);
      padding: var(--space-2) var(--space-4);
      border: 1px solid transparent;
      border-radius: var(--radius-md);
      font-size: var(--text-sm);
      font-weight: 600;
      cursor: pointer;
      transition: all var(--duration-fast) var(--ease-out);

      &:disabled {
        opacity: 0.5;
        cursor: not-allowed;
      }

      &--primary {
        background: var(--color-accent);
        color: white;
        &:hover:not(:disabled) { background: var(--color-accent-dark); }
      }

      &--accent {
        background: linear-gradient(135deg, var(--color-accent), var(--color-accent-dark));
        color: white;
        &:hover:not(:disabled) { filter: brightness(1.1); }
      }

      &--outline {
        background: transparent;
        color: var(--color-ink);
        border-color: var(--color-border);
        &:hover:not(:disabled) { border-color: var(--color-accent); color: var(--color-accent); }
      }

      &--sm { padding: var(--space-1) var(--space-3); font-size: var(--text-xs); }
      &--lg { padding: var(--space-3) var(--space-6); font-size: var(--text-base); }
    }

    // ─── Spinner ───────────────────────────────────────────
    .spinner {
      width: 16px;
      height: 16px;
      border: 2px solid rgba(255,255,255,0.3);
      border-top-color: white;
      border-radius: 50%;
      animation: spin 0.6s linear infinite;

      &--sm { width: 14px; height: 14px; }
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }
  `]
})
export class CreateListingComponent implements OnInit {
  // Categories
  categoryTree: CategoryWithChildren[] = [];
  parentCategories: CategoryWithChildren[] = [];
  childCategories: Category[] = [];
  selectedParentId = '';

  // Attributes
  attributeDefinitions: AttributeDefinition[] = [];
  attributeValues: Record<string, string> = {};

  // Regions
  regions: Region[] = [];

  // Form
  form = {
    title: '',
    shortDescription: '',
    description: '',
    categoryId: '',
    regionId: '',
    town: '',
    priceInfo: '',
    phone: '',
    email: '',
    website: '',
    address: ''
  };

  // AI
  aiInput = '';
  aiLanguage = 'fr';
  aiLoading = false;
  aiResult: AiGeneratedListing | null = null;
  aiError = '';

  // Submit
  submitting = false;

  constructor(
    private categoryService: CategoryService,
    private regionService: RegionService,
    private listingService: ListingService,
    private aiListingService: AiListingService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.categoryService.getTree().subscribe(tree => {
      this.categoryTree = tree;
      this.parentCategories = tree;
      this.cdr.markForCheck();
    });

    this.regionService.getAll().subscribe(regions => {
      this.regions = regions;
      this.cdr.markForCheck();
    });
  }

  onParentCategoryChange(parentId: string): void {
    const parent = this.categoryTree.find(c => c.id === parentId);
    this.childCategories = parent?.children ?? [];
    this.form.categoryId = '';
    this.attributeDefinitions = [];
    this.attributeValues = {};
    this.cdr.markForCheck();
  }

  onCategoryChange(categoryId: string): void {
    if (!categoryId) {
      this.attributeDefinitions = [];
      this.attributeValues = {};
      this.cdr.markForCheck();
      return;
    }

    this.categoryService.getAttributes(categoryId).subscribe(attrs => {
      this.attributeDefinitions = attrs;
      this.attributeValues = {};
      this.cdr.markForCheck();
    });
  }

  getAttributeValue(attrId: string): string {
    return this.attributeValues[attrId] ?? '';
  }

  setAttributeValue(attrId: string, value: string): void {
    this.attributeValues[attrId] = value;
  }

  isMultiSelected(attrId: string, option: string): boolean {
    const val = this.attributeValues[attrId] ?? '';
    const selected = val ? val.split(',') : [];
    return selected.includes(option);
  }

  toggleMultiSelect(attrId: string, option: string): void {
    const val = this.attributeValues[attrId] ?? '';
    const selected = val ? val.split(',') : [];
    const idx = selected.indexOf(option);
    if (idx >= 0) {
      selected.splice(idx, 1);
    } else {
      selected.push(option);
    }
    this.attributeValues[attrId] = selected.join(',');
  }

  generateWithAi(): void {
    if (!this.form.categoryId || !this.aiInput.trim()) return;

    this.aiLoading = true;
    this.aiError = '';
    this.aiResult = null;
    this.cdr.markForCheck();

    this.aiListingService.generate({
      categoryId: this.form.categoryId,
      userInput: this.aiInput.trim(),
      language: this.aiLanguage
    }).subscribe({
      next: result => {
        this.aiResult = result;
        this.aiLoading = false;
        this.cdr.markForCheck();
      },
      error: err => {
        this.aiError = err?.error?.message || err?.message || 'AI generation failed. Please try again.';
        this.aiLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  improveWithAi(): void {
    if (!this.form.categoryId) return;

    this.aiLoading = true;
    this.aiError = '';
    this.aiResult = null;
    this.cdr.markForCheck();

    this.aiListingService.improve({
      categoryId: this.form.categoryId,
      title: this.form.title,
      description: this.form.description,
      language: this.aiLanguage
    }).subscribe({
      next: result => {
        this.aiResult = result;
        this.aiLoading = false;
        this.cdr.markForCheck();
      },
      error: err => {
        this.aiError = err?.error?.message || err?.message || 'AI improvement failed. Please try again.';
        this.aiLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  applyAiResult(): void {
    if (!this.aiResult) return;

    this.form.title = this.aiResult.title;
    this.form.shortDescription = this.aiResult.shortDescription;
    this.form.description = this.aiResult.description;

    if (this.aiResult.suggestedPriceMin && this.aiResult.suggestedPriceMax) {
      this.form.priceInfo = `CHF ${this.aiResult.suggestedPriceMin} - ${this.aiResult.suggestedPriceMax}`;
    }

    for (const attr of this.aiResult.attributes) {
      this.attributeValues[attr.attributeDefinitionId] = attr.value;
    }

    this.cdr.markForCheck();
  }

  submitListing(): void {
    if (!this.form.title || !this.form.categoryId) return;

    this.submitting = true;
    this.cdr.markForCheck();

    const dto: CreateListingRequest = {
      title: this.form.title,
      shortDescription: this.form.shortDescription || undefined,
      description: this.form.description || undefined,
      categoryId: this.form.categoryId,
      regionId: this.form.regionId || undefined,
      town: this.form.town || undefined,
      isFeatured: false,
      isPremium: false,
      weight: 0,
      detail: {
        phone: this.form.phone || undefined,
        email: this.form.email || undefined,
        website: this.form.website || undefined,
        address: this.form.address || undefined,
        priceInfo: this.form.priceInfo || undefined
      },
      attributes: Object.entries(this.attributeValues)
        .filter(([, value]) => value)
        .map(([attrDefId, value]) => ({ attributeDefinitionId: attrDefId, value }))
    };

    this.listingService.create(dto).subscribe({
      next: () => {
        this.router.navigate(['/member/listings']);
      },
      error: err => {
        this.submitting = false;
        this.cdr.markForCheck();
      }
    });
  }
}
