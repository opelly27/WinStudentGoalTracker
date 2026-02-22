import { computed, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, EMPTY, Observable, of, Subject, tap } from 'rxjs';
import {
  AuthUser,
  LoginResponse,
  ResponseResult,
  SelectProgramResponse,
  TokenRefreshResponse,
  UserProgramSummary,
} from '../models/auth.models';
import { Api } from './api';

const STORAGE_KEYS = {
  JWT: 'auth_jwt',
  REFRESH_TOKEN: 'auth_refresh_token',
  SESSION_TOKEN: 'auth_session_token',
  USER: 'auth_user',
} as const;

// Refresh the JWT this many seconds before it actually expires.
const REFRESH_BUFFER_SECONDS = 60;

@Injectable({
  providedIn: 'root',
})
export class Auth {
  private readonly api = inject(Api);
  private readonly router = inject(Router);

  // --------------- Reactive state (signals) ---------------

  private readonly _user = signal<AuthUser | null>(this.loadUser());
  private readonly _sessionToken = signal<string | null>(this.loadSessionToken());
  private readonly _programs = signal<UserProgramSummary[]>([]);
  private readonly _isRefreshing = signal(false);

  /** The currently authenticated user (null when logged out). */
  readonly user = this._user.asReadonly();

  /** True when the user has completed both login phases and holds a valid JWT. */
  readonly isAuthenticated = computed(() => this._user() !== null && !!this.jwt);

  /** True while login phase 1 has succeeded and the user is choosing a program. */
  readonly isSelectingProgram = computed(() => this._sessionToken() !== null);

  /** Programs returned by phase 1 for the user to choose from. */
  readonly programs = this._programs.asReadonly();

  /** Emits when a token refresh fails and the user is forced to re-login. */
  readonly sessionExpired$ = new Subject<void>();

  private refreshTimer: ReturnType<typeof setTimeout> | null = null;

  // --------------- Accessors ---------------

  /** Current JWT (access token). */
  get jwt(): string | null {
    return localStorage.getItem(STORAGE_KEYS.JWT);
  }

  /** Current session token (phase 1 only). */
  get sessionToken(): string | null {
    return localStorage.getItem(STORAGE_KEYS.SESSION_TOKEN);
  }

  /** Current refresh token. */
  get refreshToken(): string | null {
    return localStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN);
  }

  /** Whether a token refresh is currently in flight. */
  get isRefreshing(): boolean {
    return this._isRefreshing();
  }

  // --------------- Phase 1: Login ---------------

  /**
   * Verify credentials. On success the response contains a short-lived
   * session token and the list of programs the user belongs to.
   *
   * After calling this, present the program list and call `selectProgram()`.
   */
  login(email: string, password: string): Observable<ResponseResult<LoginResponse>> {
    return this.api.login({ email, password }).pipe(
      tap((res) => {
        if (res.success && res.data) {
          localStorage.setItem(STORAGE_KEYS.SESSION_TOKEN, res.data.sessionToken);
          this._sessionToken.set(res.data.sessionToken);
          this._programs.set(res.data.programs);
        }
      }),
    );
  }

  // --------------- Phase 2: Select Program ---------------

  /**
   * Complete login by choosing a program. On success the service stores
   * the JWT + refresh token, starts the proactive refresh timer,
   * and populates the `user` signal.
   */
  selectProgram(programId: string): Observable<ResponseResult<SelectProgramResponse>> {
    return this.api.selectProgram({ programId }).pipe(
      tap((res) => {
        if (res.success && res.data) {
          this.handleFullAuth(res.data);
        }
      }),
    );
  }

  // --------------- Token Refresh ---------------

  /**
   * Manually trigger a token refresh. Normally you don't need this —
   * the proactive timer and the 401 interceptor handle it automatically.
   *
   * Returns the API response so callers can react to failures.
   */
  doRefresh(): Observable<ResponseResult<TokenRefreshResponse>> {
    const token = this.refreshToken;
    if (!token) {
      this.forceLogout();
      return of({ success: false, message: 'No refresh token.' });
    }

    if (this._isRefreshing()) {
      return EMPTY;
    }

    this._isRefreshing.set(true);

    return this.api.refreshToken({ refreshToken: token }).pipe(
      tap((res) => {
        this._isRefreshing.set(false);
        if (res.success && res.data) {
          this.storeTokens(res.data.jwt, res.data.newRefreshToken);
          this.scheduleRefresh(res.data.jwtExpiresIn);
        } else {
          this.forceLogout();
        }
      }),
      catchError(() => {
        this._isRefreshing.set(false);
        this.forceLogout();
        return of({ success: false, message: 'Token refresh failed.' } as ResponseResult<TokenRefreshResponse>);
      }),
    );
  }

  // --------------- Logout ---------------

  /** Log out: revoke the refresh token on the server, then clear local state. */
  logout(): Observable<ResponseResult<object>> {
    const token = this.refreshToken;
    this.clearState();

    if (!token) {
      return of({ success: true, message: 'Logged out.' });
    }

    return this.api.logout({ refreshToken: token }).pipe(
      catchError(() => of({ success: true, message: 'Logged out locally.' })),
    );
  }

  // --------------- Internals ---------------

  /**
   * Called by the 401 interceptor when a refresh fails irrecoverably.
   * Clears all auth state and redirects to login.
   */
  forceLogout(): void {
    this.clearState();
    this.sessionExpired$.next();
    this.router.navigateByUrl('/login');
  }

  private handleFullAuth(data: SelectProgramResponse): void {
    const user: AuthUser = {
      userId: data.userId,
      email: data.email,
      programId: data.jwt ? this.extractClaim(data.jwt, 'program_id') ?? '' : '',
      programName: data.programName,
      role: data.role,
      roleDisplayName: data.roleDisplayName,
    };

    // Persist
    this.storeTokens(data.jwt, data.refreshToken);
    localStorage.setItem(STORAGE_KEYS.USER, JSON.stringify(user));

    // Clear phase-1 artefacts
    localStorage.removeItem(STORAGE_KEYS.SESSION_TOKEN);
    this._sessionToken.set(null);
    this._programs.set([]);

    // Update reactive state
    this._user.set(user);

    // Start proactive refresh
    this.scheduleRefresh(data.jwtExpiresIn);
  }

  private storeTokens(jwt: string, refreshToken: string): void {
    localStorage.setItem(STORAGE_KEYS.JWT, jwt);
    localStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, refreshToken);
  }

  private scheduleRefresh(expiresInSeconds: number): void {
    this.clearRefreshTimer();
    const delayMs = Math.max((expiresInSeconds - REFRESH_BUFFER_SECONDS) * 1000, 0);
    this.refreshTimer = setTimeout(() => {
      this.doRefresh().subscribe();
    }, delayMs);
  }

  private clearRefreshTimer(): void {
    if (this.refreshTimer !== null) {
      clearTimeout(this.refreshTimer);
      this.refreshTimer = null;
    }
  }

  private clearState(): void {
    this.clearRefreshTimer();
    localStorage.removeItem(STORAGE_KEYS.JWT);
    localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN);
    localStorage.removeItem(STORAGE_KEYS.SESSION_TOKEN);
    localStorage.removeItem(STORAGE_KEYS.USER);
    this._user.set(null);
    this._sessionToken.set(null);
    this._programs.set([]);
    this._isRefreshing.set(false);
  }

  private loadUser(): AuthUser | null {
    try {
      const raw = localStorage.getItem(STORAGE_KEYS.USER);
      return raw ? (JSON.parse(raw) as AuthUser) : null;
    } catch {
      return null;
    }
  }

  private loadSessionToken(): string | null {
    return localStorage.getItem(STORAGE_KEYS.SESSION_TOKEN);
  }

  /**
   * Decode a single claim from a JWT without pulling in a library.
   * Returns null if the claim is missing or the token is malformed.
   */
  private extractClaim(jwt: string, claim: string): string | null {
    try {
      const payload = jwt.split('.')[1];
      const decoded = JSON.parse(atob(payload));
      return decoded[claim] ?? null;
    } catch {
      return null;
    }
  }
}
