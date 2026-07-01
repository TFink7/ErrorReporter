import { Routes } from '@angular/router';
import { apiKeyGuard } from './core/api-key.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  {
    path: 'dashboard',
    canActivate: [apiKeyGuard],
    loadComponent: () => import('./pages/dashboard/dashboard').then(m => m.DashboardPage),
  },
  {
    path: 'errors',
    canActivate: [apiKeyGuard],
    loadComponent: () => import('./pages/errors/error-list').then(m => m.ErrorListPage),
  },
  {
    path: 'errors/:id',
    canActivate: [apiKeyGuard],
    loadComponent: () => import('./pages/errors/error-detail').then(m => m.ErrorDetailPage),
  },
  {
    path: 'clients',
    canActivate: [apiKeyGuard],
    loadComponent: () => import('./pages/clients/clients').then(m => m.ClientsPage),
  },
  {
    path: 'settings',
    loadComponent: () => import('./pages/settings/settings').then(m => m.SettingsPage),
  },
  { path: '**', redirectTo: 'dashboard' },
];
