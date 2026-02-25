import { Routes } from '@angular/router';

export const PUBLIC_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./home/home.component').then(m => m.HomeComponent)
  },
  {
    path: 'listings',
    loadComponent: () => import('./listing-grid/listing-grid.component').then(m => m.ListingGridComponent)
  },
  {
    path: 'listings/:id',
    loadComponent: () => import('./listing-detail/listing-detail.component').then(m => m.ListingDetailComponent)
  },
  {
    path: 'privacy',
    loadComponent: () => import('./privacy/privacy.component').then(m => m.PrivacyComponent)
  },
  {
    path: 'terms',
    loadComponent: () => import('./terms/terms.component').then(m => m.TermsComponent)
  }
];
