import { ApiResponse } from '../models/api.models';

export function unwrapApiResponse<T>(response: ApiResponse<T>): T {
  if (!response.success) {
    throw new Error(response.message || 'Request failed');
  }

  return response.data as T;
}
