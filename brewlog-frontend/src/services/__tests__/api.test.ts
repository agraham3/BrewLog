// Simple test to verify API client functionality
// This would normally use a testing framework like Jest or Vitest

import { apiClient, ApiClientError } from '../api';

// Mock fetch for testing
const originalFetch = global.fetch;

const mockFetch = (response: any, ok: boolean = true, status: number = 200) => {
  global.fetch = jest.fn(() =>
    Promise.resolve({
      ok,
      status,
      json: () => Promise.resolve(response),
      headers: new Headers({ 'content-type': 'application/json' }),
    })
  ) as jest.Mock;
};

const restoreFetch = () => {
  global.fetch = originalFetch;
};

describe('API Client', () => {
  afterEach(() => {
    restoreFetch();
  });

  test('should handle successful GET request', async () => {
    const mockData = { id: 1, name: 'Test Bean' };
    mockFetch(mockData);

    const result = await apiClient.get('/coffeebeans/1');
    expect(result).toEqual(mockData);
  });

  test('should handle API error responses', async () => {
    const mockError = {
      code: 'VALIDATION_ERROR',
      message: 'Invalid data',
      details: [{ field: 'name', message: 'Name is required' }]
    };
    mockFetch(mockError, false, 400);

    await expect(apiClient.get('/coffeebeans/1')).rejects.toThrow(ApiClientError);
  });

  test('should handle query parameters', async () => {
    const mockData = [{ id: 1, name: 'Test Bean' }];
    mockFetch(mockData);

    await apiClient.get('/coffeebeans', { name: 'test', roastLevel: 'Medium' });
    
    expect(global.fetch).toHaveBeenCalledWith(
      expect.stringContaining('name=test&roastLevel=Medium'),
      expect.any(Object)
    );
  });
});

// Export for potential manual testing
export const testApiClient = {
  mockFetch,
  restoreFetch,
};