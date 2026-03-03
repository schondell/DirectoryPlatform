import { Routes } from '@angular/router';
import { authGuard } from '../../core/guards/auth.guard';

export const MEMBER_ROUTES: Routes = [
  {
    path: '',
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', loadComponent: () => import('./dashboard/dashboard.component').then(m => m.MemberDashboardComponent) },
      { path: 'listings', loadComponent: () => import('./listings/listings.component').then(m => m.MemberListingsComponent) },
      { path: 'listings/create', loadComponent: () => import('./create-listing/create-listing.component').then(m => m.CreateListingComponent) },
      { path: 'messages', loadComponent: () => import('./messages/messages.component').then(m => m.MessagesComponent) },
      { path: 'notifications', loadComponent: () => import('./notifications/notifications.component').then(m => m.NotificationsComponent) },
      { path: 'profile', loadComponent: () => import('./profile/profile.component').then(m => m.ProfileComponent) },
      { path: 'settings', loadComponent: () => import('./settings/settings.component').then(m => m.SettingsComponent) },
      { path: 'subscription', loadComponent: () => import('./subscription/subscription.component').then(m => m.SubscriptionComponent) }
    ]
  }
];
