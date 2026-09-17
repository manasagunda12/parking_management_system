import { Routes } from '@angular/router';
import { authGuard } from './Guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./Components/login-component/login-component')
        .then(m => m.LoginComponent)
  },

  {
    path: 'register',
    loadComponent: () =>
      import('./Components/register-component/register-component')
        .then(m => m.RegisterComponent)
  },

  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./Components/dashboard/dashboard')
        .then(m => m.Dashboard)
  },

  {
    path: 'admin',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./Components/admin-component/admin-component')
        .then(m => m.AdminComponent)
  },

  {
    path: 'operator',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./Components/operator-component/operator-component')
        .then(m => m.OperatorComponent)
  },

  {
    path: '**',
    redirectTo: ''
  }
];