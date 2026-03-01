import { HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { Auth } from '../services/auth';

/**
 * Functional HTTP interceptor that:
 *  1. Attaches the current JWT (or session token during phase 1) to outgoing requests.
 *  2. On a 401 response, attempts a single token refresh then retries the original request.
 *  3. If the refresh also fails, forces logout.
 */
export const authInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
) => {
  const auth = inject(Auth);

  // Skip token attachment for the refresh-token endpoint to avoid circular 401 loops.
  const isRefreshRequest = req.url.includes('/Auth/RefreshToken');
  const cloned = isRefreshRequest ? req : attachToken(req, auth);

  return next(cloned).pipe(
    catchError((error) => {
      if (error.status === 401 && !isRefreshRequest && auth.refreshToken) {
        return auth.doRefresh().pipe(
          switchMap((res) => {
            if (res.success) {
              return next(attachToken(req, auth));
            }
            return throwError(() => error);
          }),
          catchError(() => throwError(() => error)),
        );
      }

      return throwError(() => error);
    }),
  );
};

function attachToken(req: HttpRequest<unknown>, auth: Auth): HttpRequest<unknown> {
  const token = auth.jwt ?? auth.sessionToken;
  if (!token) return req;

  return req.clone({
    setHeaders: { Authorization: `Bearer ${token}` },
  });
}
