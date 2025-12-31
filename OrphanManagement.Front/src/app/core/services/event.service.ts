import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResult } from '../models/api.models';
import { CreateEventRequest, EventDto, UpdateEventRequest } from '../models/event.models';
import { unwrapApiResponse } from './api-helpers';

@Injectable({ providedIn: 'root' })
export class EventService {
  constructor(private readonly http: HttpClient) {}

  getEvents(options: { pageNumber: number; pageSize: number; searchTerm?: string | null }) {
    let params = new HttpParams()
      .set('pageNumber', String(options.pageNumber))
      .set('pageSize', String(options.pageSize));

    if (options.searchTerm) {
      params = params.set('searchTerm', options.searchTerm);
    }

    return this.http
      .get<ApiResponse<PagedResult<EventDto>>>(`${environment.apiBaseUrl}/Events`, { params })
      .pipe(map(unwrapApiResponse));
  }

  createEvent(request: CreateEventRequest) {
    return this.http
      .post<ApiResponse<EventDto>>(`${environment.apiBaseUrl}/Events`, request)
      .pipe(map(unwrapApiResponse));
  }

  updateEvent(id: string, request: UpdateEventRequest) {
    return this.http
      .put<ApiResponse<EventDto>>(`${environment.apiBaseUrl}/Events/${id}`, request)
      .pipe(map(unwrapApiResponse));
  }

  deleteEvent(id: string) {
    return this.http.delete(`${environment.apiBaseUrl}/Events/${id}`);
  }

  getUpcomingEvents(count = 5) {
    const params = new HttpParams().set('count', String(count));

    return this.http
      .get<ApiResponse<EventDto[]>>(`${environment.apiBaseUrl}/Events/upcoming`, { params })
      .pipe(map(unwrapApiResponse));
  }

  getStatsByMonth(year?: number) {
    let params = new HttpParams();
    if (year) params = params.set('year', String(year));

    return this.http
      .get<ApiResponse<Record<string, number>>>(`${environment.apiBaseUrl}/Events/statistics/by-month`, {
        params
      })
      .pipe(map(unwrapApiResponse));
  }
}
