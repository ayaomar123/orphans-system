import { Injectable, computed, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api.models';
import { LoginRequest, LoginResponse } from '../models/auth.models';
import { unwrapApiResponse } from './api-helpers';

const STORAGE_KEY = 'om.auth';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly authSignal = signal<LoginResponse | null>(this.load());

  readonly user = computed(() => this.authSignal());
  readonly token = computed(() => this.authSignal()?.token ?? null);
  readonly role = computed(() => this.authSignal()?.role ?? null);
  readonly isLoggedIn = computed(() => !!this.authSignal()?.token);

  constructor(private readonly http: HttpClient) {}

  login(request: LoginRequest) {
    return this.http
      .post<ApiResponse<LoginResponse>>(`${environment.apiBaseUrl}/Auth/login`, request)
      .pipe(
        map(unwrapApiResponse),
        tap((auth) => this.setAuth(auth))
      );
  }

  logout() {
    localStorage.removeItem(STORAGE_KEY);
    this.authSignal.set(null);
  }

  private setAuth(auth: LoginResponse) {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(auth));
    this.authSignal.set(auth);
  }

  private load(): LoginResponse | null {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (!raw) return null;
      return JSON.parse(raw) as LoginResponse;
    } catch {
      return null;
    }
  }
}
