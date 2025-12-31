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
  readonly isLoggedIn = computed(() => {
    const auth = this.authSignal();
    return !!auth?.token;
  });

  constructor(private readonly http: HttpClient) {
    console.log('AuthService initialized');
    this.verifyLocalStorage();
  }

  login(request: LoginRequest) {
    return this.http
      .post<ApiResponse<LoginResponse>>(`${environment.apiBaseUrl}/Auth/login`, request)
      .pipe(
        map(unwrapApiResponse),
        tap((auth) => {
          console.log('Login successful, storing auth data', {
            hasToken: !!auth.token,
            tokenLength: auth.token?.length,
            expiresAt: auth.expiresAt,
            email: auth.email
          });
          this.setAuth(auth);
        })
      );
  }

  logout() {
    console.log('Logging out, clearing auth data');
    try {
      localStorage.removeItem(STORAGE_KEY);
      this.authSignal.set(null);
    } catch (error) {
      console.error('Error during logout:', error);
    }
  }

  private setAuth(auth: LoginResponse) {
    try {
      if (!auth || !auth.token) {
        console.error('Invalid auth data received:', auth);
        return;
      }

      const authData = JSON.stringify(auth);
      localStorage.setItem(STORAGE_KEY, authData);
      console.log('Auth data stored in localStorage, length:', authData.length);
      
      this.authSignal.set(auth);
      
      setTimeout(() => {
        const stored = localStorage.getItem(STORAGE_KEY);
        if (!stored) {
          console.error('WARNING: Auth data was cleared from localStorage immediately after saving!');
        } else if (stored !== authData) {
          console.warn('WARNING: Auth data in localStorage differs from what was saved!');
        } else {
          console.log('Verified: Auth data persisted in localStorage');
        }
      }, 100);
    } catch (error) {
      console.error('Error storing auth data:', error);
      if (error instanceof DOMException && error.name === 'QuotaExceededError') {
        console.error('localStorage quota exceeded!');
      }
    }
  }

  private load(): LoginResponse | null {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (!raw) {
        console.log('No auth data found in localStorage');
        return null;
      }
      
      const auth = JSON.parse(raw) as LoginResponse;
      console.log('Loaded auth data from localStorage:', {
        hasToken: !!auth.token,
        tokenLength: auth.token?.length,
        expiresAt: auth.expiresAt,
        email: auth.email
      });
      
      if (auth.expiresAt) {
        const expiresAt = new Date(auth.expiresAt);
        const now = new Date();
        if (expiresAt <= now) {
          console.warn('Stored token has expired, clearing it');
          localStorage.removeItem(STORAGE_KEY);
          return null;
        }
      }
      
      return auth;
    } catch (error) {
      console.error('Error loading auth data from localStorage:', error);
      localStorage.removeItem(STORAGE_KEY);
      return null;
    }
  }

  private verifyLocalStorage() {
    try {
      const testKey = 'om.test';
      localStorage.setItem(testKey, 'test');
      const value = localStorage.getItem(testKey);
      localStorage.removeItem(testKey);
      
      if (value !== 'test') {
        console.error('localStorage is not working correctly!');
      } else {
        console.log('localStorage is working correctly');
      }
    } catch (error) {
      console.error('localStorage is not available:', error);
    }
  }
}
