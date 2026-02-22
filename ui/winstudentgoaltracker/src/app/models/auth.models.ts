// Generic API response wrapper — matches C# ResponseResult<T>
export interface ResponseResult<T> {
  success: boolean;
  message: string;
  data?: T;
}

// Phase 1: Login
export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  sessionToken: string;
  programs: UserProgramSummary[];
}

export interface UserProgramSummary {
  programId: string;
  programName: string;
  role: string;
  roleDisplayName: string;
  isPrimary: boolean;
}

// Phase 2: Select Program
export interface SelectProgramRequest {
  programId: string;
}

export interface SelectProgramResponse {
  userId: string;
  email: string;
  programName: string;
  jwt: string;
  refreshToken: string;
  role: string;
  roleDisplayName: string;
  jwtExpiresIn: number;
}

// Token Refresh
export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface TokenRefreshResponse {
  jwt: string;
  newRefreshToken: string;
  jwtExpiresIn: number;
}

// Auth state exposed by the Auth service
export interface AuthUser {
  userId: string;
  email: string;
  programId: string;
  programName: string;
  role: string;
  roleDisplayName: string;
}
