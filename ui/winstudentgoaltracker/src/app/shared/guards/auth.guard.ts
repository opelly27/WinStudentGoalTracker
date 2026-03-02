import { inject } from '@angular/core';
import { Router, UrlTree } from '@angular/router';
import { Auth } from '../services/auth';

// *****************************************************************
// Route guard that checks if the user is logged in. If not, they
// get redirected to the login page. Used on all routes that require
// an authenticated session (desktop and mobile home, etc.).
// *****************************************************************
export function authGuard(): boolean | UrlTree {
    const auth = inject(Auth);
    const router = inject(Router);

    if (auth.isAuthenticated()) {
        return true;
    }

    return router.createUrlTree(['/login']);
}
