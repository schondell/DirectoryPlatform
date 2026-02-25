import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { SubscriptionService } from '../../../core/services/subscription.service';
import { SubscriptionTier } from '../../../core/models';

@Component({
  selector: 'app-subscription',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  template: `
    <h1>{{ 'subscription.plans' | translate }}</h1>
    <div class="tiers">
      @for (tier of tiers; track tier.id) {
        <div class="tier-card">
          <h3>{{ tier.name }}</h3>
          <p>{{ tier.description }}</p>
          <p class="price">{{ tier.monthlyPrice }} CHF / {{ 'subscription.monthly' | translate }}</p>
          <ul>@for (f of tier.features; track f.id) { <li>{{ f.name }}: {{ f.value }}</li> }</ul>
        </div>
      }
    </div>
  `
})
export class SubscriptionComponent implements OnInit {
  tiers: SubscriptionTier[] = [];
  constructor(private subscriptionService: SubscriptionService) {}
  ngOnInit(): void { this.subscriptionService.getTiers().subscribe(t => this.tiers = t); }
}
