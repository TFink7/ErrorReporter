import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  ApiClient,
  CreatedApiClient,
  ErrorFilter,
  ErrorReport,
  ErrorSummary,
  PagedResult,
} from './models';

const BASE_URL = 'http://localhost:5087';

@Injectable({ providedIn: 'root' })
export class ErrorApiService {
  private readonly http = inject(HttpClient);

  getErrors(filter: ErrorFilter): Observable<PagedResult<ErrorReport>> {
    let params = new HttpParams();

    if (filter.service) params = params.set('service', filter.service);
    if (filter.from) params = params.set('from', filter.from);
    if (filter.to) params = params.set('to', filter.to);
    if (filter.page) params = params.set('page', filter.page);
    if (filter.pageSize) params = params.set('pageSize', filter.pageSize);

    return this.http.get<PagedResult<ErrorReport>>(`${BASE_URL}/errors`, { params });
  }

  getError(id: number): Observable<ErrorReport> {
    return this.http.get<ErrorReport>(`${BASE_URL}/errors/${id}`);
  }

  deleteError(id: number): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/errors/${id}`);
  }

  getSummary(): Observable<ErrorSummary[]> {
    return this.http.get<ErrorSummary[]>(`${BASE_URL}/errors/summary`);
  }

  getClients(): Observable<ApiClient[]> {
    return this.http.get<ApiClient[]>(`${BASE_URL}/admin/clients`);
  }

  createClient(name: string): Observable<CreatedApiClient> {
    return this.http.post<CreatedApiClient>(`${BASE_URL}/admin/clients`, { name });
  }

  revokeClient(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${BASE_URL}/admin/clients/${id}`);
  }
}
