import { Injectable, signal } from '@angular/core';

const TOKEN_KEY = 'tmui_jwt_token';

/**
 * Minimal JWT holder. The backend README describes how to obtain a token
 * (e.g. a POST /api/auth/login or a seeded dev token). Paste the token
 * into the "JWT Token" field in the header - it will be attached as a
 * Bearer token to every API request by the AuthInterceptor.
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  readonly token = signal<string | null>(localStorage.getItem(TOKEN_KEY));

  setToken(token: string): void {
    localStorage.setItem(TOKEN_KEY, token);
    this.token.set(token);
  }

  clearToken(): void {
    localStorage.removeItem(TOKEN_KEY);
    this.token.set(null);
  }

  get isAuthenticated(): boolean {
    return !!this.token();
  }
}
