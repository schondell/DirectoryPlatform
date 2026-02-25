import { Routes } from '@angular/router';
import { roleGuard } from '../../core/guards/auth.guard';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    canActivate: [roleGuard(['Admin', 'SuperAdmin'])],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', loadComponent: () => import('./dashboard/dashboard.component').then(m => m.AdminDashboardComponent) },
      { path: 'approval', loadComponent: () => import('./approval/approval.component').then(m => m.ApprovalComponent) },
      { path: 'categories', loadComponent: () => import('./categories/categories.component').then(m => m.AdminCategoriesComponent) },
      { path: 'attribute-definitions', loadComponent: () => import('./attribute-definitions/attribute-definitions.component').then(m => m.AttributeDefinitionsComponent) },
      { path: 'audit', loadComponent: () => import('./audit/audit.component').then(m => m.AuditComponent) }
    ]
  }
];
