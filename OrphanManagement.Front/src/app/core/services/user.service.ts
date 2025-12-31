import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResult } from '../models/api.models';
import { CreateUserRequest, UpdateUserRequest, UserDto } from '../models/user.models';
import { unwrapApiResponse } from './api-helpers';

@Injectable({ providedIn: 'root' })
export class UserService {
  constructor(private readonly http: HttpClient) {}

  getUsers(options: { pageNumber: number; pageSize: number; searchTerm?: string | null; role?: string | null }) {
    let params = new HttpParams()
      .set('pageNumber', String(options.pageNumber))
      .set('pageSize', String(options.pageSize));

    if (options.searchTerm) params = params.set('searchTerm', options.searchTerm);
    if (options.role) params = params.set('role', options.role);

    return this.http
      .get<ApiResponse<PagedResult<UserDto>>>(`${environment.apiBaseUrl}/Users`, { params })
      .pipe(map(unwrapApiResponse));
  }

  createUser(request: CreateUserRequest) {
    return this.http
      .post<ApiResponse<UserDto>>(`${environment.apiBaseUrl}/Users`, request)
      .pipe(map(unwrapApiResponse));
  }

  updateUser(id: string, request: UpdateUserRequest) {
    return this.http
      .put<ApiResponse<UserDto>>(`${environment.apiBaseUrl}/Users/${id}`, request)
      .pipe(map(unwrapApiResponse));
  }

  deleteUser(id: string) {
    return this.http.delete(`${environment.apiBaseUrl}/Users/${id}`);
  }
}
