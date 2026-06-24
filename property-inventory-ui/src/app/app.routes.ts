import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent)
  },
  {
    path: 'properties',
    loadComponent: () =>
      import('./features/properties/property-list/property-list.component').then((m) => m.PropertyListComponent)
  },
  {
    path: 'properties/new',
    loadComponent: () =>
      import('./features/properties/property-form/property-form.component').then((m) => m.PropertyFormComponent)
  },
  {
    path: 'properties/:id/edit',
    loadComponent: () =>
      import('./features/properties/property-form/property-form.component').then((m) => m.PropertyFormComponent)
  },
  {
    path: 'properties/:id',
    loadComponent: () =>
      import('./features/properties/property-detail/property-detail.component').then((m) => m.PropertyDetailComponent)
  },
  {
    path: 'contacts',
    loadComponent: () =>
      import('./features/contacts/contact-list/contact-list.component').then((m) => m.ContactListComponent)
  },
  {
    path: 'contacts/new',
    loadComponent: () =>
      import('./features/contacts/contact-form/contact-form.component').then((m) => m.ContactFormComponent)
  },
  {
    path: 'contacts/:id/edit',
    loadComponent: () =>
      import('./features/contacts/contact-form/contact-form.component').then((m) => m.ContactFormComponent)
  },
  {
    path: 'contacts/:id',
    loadComponent: () =>
      import('./features/contacts/contact-detail/contact-detail.component').then((m) => m.ContactDetailComponent)
  },
  { path: '**', redirectTo: 'dashboard' }
];
