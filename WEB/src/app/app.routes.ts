import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: 'login', loadComponent: () => import('./components/authentication/login.component').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () => import('./components/authentication/register.component').then(m => m.RegisterComponent) },
  { path: 'offers', loadComponent: () => import('./components/offer/offer-list.component').then(m => m.OfferListComponent) },
  { path: 'offers/:id', loadComponent: () => import('./components/offer/offer-detail.component').then(m => m.OfferDetailComponent) },
  { path: '', redirectTo: 'offers', pathMatch: 'full' },
  { path: '**', redirectTo: 'offers' }
];
