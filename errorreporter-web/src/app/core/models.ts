export type Severity = 'Info' | 'Warning' | 'Error' | 'Critical';

export const SEVERITIES: Severity[] = ['Info', 'Warning', 'Error', 'Critical'];

export interface ErrorReport {
  id: number;
  service: string;
  message: string;
  stackTrace: string | null;
  severity: Severity;
  occurredAt: string;
  createdAt: string;
  apiClientId: number | null;
}

export interface PagedResult<T> {
  data: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface ErrorSummary {
  service: string;
  count: number;
  mostRecentOccurrence: string | null;
}

export interface ApiClient {
  id: number;
  name: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreatedApiClient {
  id: number;
  name: string;
  apiKey: string;
  message: string;
}

export interface ErrorFilter {
  service?: string;
  from?: string;
  to?: string;
  page?: number;
  pageSize?: number;
}
