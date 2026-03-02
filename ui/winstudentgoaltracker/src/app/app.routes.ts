import { inject } from '@angular/core';
import { Routes } from '@angular/router';
import { Login } from './shared/pages/login/login';
import { PlatformService } from './shared/services/platform.service';
import { authGuard } from './shared/guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: Login },
  {
    path: '',
    canMatch: [() => inject(PlatformService).formFactor() === 'mobile'],
    canActivate: [authGuard],
    loadChildren: () => import('./mobile/mobile.routes'),
  },
  {
    path: '',
    canActivate: [authGuard],
    loadChildren: () => import('./desktop/desktop.routes'),
  },
];
