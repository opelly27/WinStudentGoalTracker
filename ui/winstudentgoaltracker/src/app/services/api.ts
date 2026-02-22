import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  LoginRequest,
  LoginResponse,
  RefreshTokenRequest,
  ResponseResult,
  SelectProgramRequest,
  SelectProgramResponse,
  TokenRefreshResponse,
} from '../models/auth.models';

@Injectable({
  providedIn: 'root',
})
export class Api {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiBaseUrl;

  // Phase 1 — verify credentials, receive session token + program list
  login(request: LoginRequest): Observable<ResponseResult<LoginResponse>> {
    return this.http.post<ResponseResult<LoginResponse>>(
      `${this.base}/api/Auth/Login`,
      request,
    );
  }

  // Phase 2 — select a program, receive JWT + refresh token
  selectProgram(request: SelectProgramRequest): Observable<ResponseResult<SelectProgramResponse>> {
    return this.http.post<ResponseResult<SelectProgramResponse>>(
      `${this.base}/api/Auth/SelectProgram`,
      request,
    );
  }

  // Exchange a refresh token for a new JWT + rotated refresh token
  refreshToken(request: RefreshTokenRequest): Observable<ResponseResult<TokenRefreshResponse>> {
    return this.http.post<ResponseResult<TokenRefreshResponse>>(
      `${this.base}/api/Auth/RefreshToken`,
      request,
    );
  }

  // Revoke the refresh token and log out
  logout(request: RefreshTokenRequest): Observable<ResponseResult<object>> {
    return this.http.post<ResponseResult<object>>(
      `${this.base}/api/Auth/Logout`,
      request,
    );
  }
}
