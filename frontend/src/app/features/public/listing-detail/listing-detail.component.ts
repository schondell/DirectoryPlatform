import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { ListingService } from '../../../core/services/listing.service';
import { Listing } from '../../../core/models';

@Component({
  selector: 'app-listing-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  template: `
    <div class="listing-detail" *ngIf="listing">
      <div class="listing-header">
        <h1>{{ listing.title }}</h1>
        <div class="listing-meta">
          <span class="category">{{ listing.category?.name }}</span>
          <span class="region" *ngIf="listing.region">{{ listing.region?.name }}</span>
          <span class="views">{{ listing.viewCount }} {{ 'listing.views' | translate }}</span>
        </div>
      </div>

      <div class="listing-gallery" *ngIf="listing.media?.length">
        @for (media of listing.media; track media.id) {
          <img [src]="media.url" [alt]="media.altText || listing.title" />
        }
      </div>

      <div class="listing-body">
        <div class="description">
          <p *ngIf="listing.shortDescription" class="short-desc">{{ listing.shortDescription }}</p>
          <div *ngIf="listing.description" [innerHTML]="listing.description"></div>
        </div>

        <div class="attributes" *ngIf="listing.attributes?.length">
          <h3>Details</h3>
          @for (attr of listing.attributes; track attr.id) {
            <div class="attr-row">
              <span class="attr-name">{{ attr.attributeName }}</span>
              <span class="attr-value">{{ attr.value }}{{ attr.unit ? ' ' + attr.unit : '' }}</span>
            </div>
          }
        </div>

        <div class="contact" *ngIf="listing.detail">
          <h3>{{ 'listing.contact' | translate }}</h3>
          <p *ngIf="listing.detail.address">{{ listing.detail.address }}</p>
          <p *ngIf="listing.detail.phone">{{ listing.detail.phone }}</p>
          <p *ngIf="listing.detail.email">{{ listing.detail.email }}</p>
          <p *ngIf="listing.detail.website"><a [href]="listing.detail.website" target="_blank">{{ listing.detail.website }}</a></p>
        </div>
      </div>
    </div>
  `
})
export class ListingDetailComponent implements OnInit {
  listing: Listing | null = null;

  constructor(
    private route: ActivatedRoute,
    private listingService: ListingService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.listingService.getById(id).subscribe(l => this.listing = l);
    }
  }
}
