import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResult } from '../models/api.models';
import { CreateOrphanRequest, OrphanDto, UpdateOrphanRequest } from '../models/orphan.models';
import { unwrapApiResponse } from './api-helpers';

@Injectable({ providedIn: 'root' })
export class OrphanService {
  constructor(private readonly http: HttpClient) {}

  getOrphans(options: { pageNumber: number; pageSize: number; searchTerm?: string | null }) {
    let params = new HttpParams()
      .set('pageNumber', String(options.pageNumber))
      .set('pageSize', String(options.pageSize));

    if (options.searchTerm) {
      params = params.set('searchTerm', options.searchTerm);
    }

    return this.http
      .get<ApiResponse<PagedResult<OrphanDto>>>(`${environment.apiBaseUrl}/Orphans`, { params })
      .pipe(map(unwrapApiResponse));
  }

  getOrphanById(id: string) {
    return this.http
      .get<ApiResponse<OrphanDto>>(`${environment.apiBaseUrl}/Orphans/${id}`)
      .pipe(map(unwrapApiResponse));
  }

  createOrphan(request: CreateOrphanRequest) {
    return this.http
      .post<ApiResponse<OrphanDto>>(`${environment.apiBaseUrl}/Orphans`, request)
      .pipe(map(unwrapApiResponse));
  }

  updateOrphan(id: string, request: UpdateOrphanRequest) {
    return this.http
      .put<ApiResponse<OrphanDto>>(`${environment.apiBaseUrl}/Orphans/${id}`, request)
      .pipe(map(unwrapApiResponse));
  }

  deleteOrphan(id: string) {
    return this.http.delete(`${environment.apiBaseUrl}/Orphans/${id}`);
  }

  getStatsByCity() {
    return this.http
      .get<ApiResponse<Record<string, number>>>(`${environment.apiBaseUrl}/Orphans/statistics/by-city`)
      .pipe(map(unwrapApiResponse));
  }

  getStatsByAgeGroup() {
    return this.http
      .get<ApiResponse<Record<string, number>>>(
        `${environment.apiBaseUrl}/Orphans/statistics/by-age-group`
      )
      .pipe(map(unwrapApiResponse));
  }
}
