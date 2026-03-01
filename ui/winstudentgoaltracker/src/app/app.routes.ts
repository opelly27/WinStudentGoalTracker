import { inject } from '@angular/core';
import { Routes } from '@angular/router';
import { Login } from './shared/pages/login/login';
import { PlatformService } from './shared/services/platform.service';

export const routes: Routes = [
  { path: 'login', component: Login },
  {
    path: '',
    canMatch: [() => inject(PlatformService).formFactor() === 'mobile'],
    loadChildren: () => import('./mobile/mobile.routes'),
  },
  {
    path: '',
    loadChildren: () => import('./desktop/desktop.routes'),
  },
];
