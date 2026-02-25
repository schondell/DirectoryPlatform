import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { ListingService } from '../../../core/services/listing.service';
import { Listing } from '../../../core/models';

@Component({
  selector: 'app-member-listings',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  template: `
    <h1>{{ 'listing.myListings' | translate }}</h1>
    <a routerLink="/member/listings/create">{{ 'listing.createListing' | translate }}</a>
    @for (listing of listings; track listing.id) {
      <div class="listing-row">
        <span>{{ listing.title }}</span>
        <span class="status">{{ listing.status }}</span>
        <a [routerLink]="['/listings', listing.id]">View</a>
      </div>
    }
    @if (listings.length === 0) { <p>{{ 'listing.noListings' | translate }}</p> }
  `
})
export class MemberListingsComponent implements OnInit {
  listings: Listing[] = [];
  constructor(private listingService: ListingService) {}
  ngOnInit(): void { this.listingService.getMyListings().subscribe(l => this.listings = l); }
}
