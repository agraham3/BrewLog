import type { ApiResponse, ApiError } from '@/types';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:7001/api';

// Custom error class for API errors
export class ApiClientError extends Error {
  public code: string;
  public details?: Array<{ field: string; message: string }>;
  public status?: number;

  constructor(message: string, code: string, status?: number, details?: Array<{ field: string; message: string }>) {
    super(message);
    this.name = 'ApiClientError';
    this.code = code;
    this.status = status;
    this.details = details;
  }
}

// Request interceptor type
type RequestInterceptor = (config: RequestInit) => RequestInit | Promise<RequestInit>;

// Response interceptor type
type ResponseInterceptor = (response: Response) => Response | Promise<Response>;

class ApiClient {
  private baseURL: string;
  private requestInterceptors: RequestInterceptor[] = [];
  private responseInterceptors: ResponseInterceptor[] = [];

  constructor(baseURL: string) {
    this.baseURL = baseURL;
    this.setupDefaultInterceptors();
  }

  private setupDefaultInterceptors() {
    // Default request interceptor for common headers
    this.addRequestInterceptor((config) => ({
      ...config,
      headers: {
        'Content-Type': 'application/json',
        ...config.headers,
      },
    }));

    // Default response interceptor for error handling
    this.addResponseInterceptor(async (response) => {
      if (!response.ok) {
        let errorData: ApiError;
        try {
          errorData = await response.json();
        } catch {
          // If response is not JSON, create a generic error
          errorData = {
            code: 'HTTP_ERROR',
            message: `HTTP error! status: ${response.status}`,
          };
        }

        throw new ApiClientError(
          errorData.message || `HTTP error! status: ${response.status}`,
          errorData.code || 'HTTP_ERROR',
          response.status,
          errorData.details
        );
      }
      return response;
    });
  }

  // Add request interceptor
  addRequestInterceptor(interceptor: RequestInterceptor): void {
    this.requestInterceptors.push(interceptor);
  }

  // Add response interceptor
  addResponseInterceptor(interceptor: ResponseInterceptor): void {
    this.responseInterceptors.push(interceptor);
  }

  private async applyRequestInterceptors(config: RequestInit): Promise<RequestInit> {
    let finalConfig = config;
    for (const interceptor of this.requestInterceptors) {
      finalConfig = await interceptor(finalConfig);
    }
    return finalConfig;
  }

  private async applyResponseInterceptors(response: Response): Promise<Response> {
    let finalResponse = response;
    for (const interceptor of this.responseInterceptors) {
      finalResponse = await interceptor(finalResponse);
    }
    return finalResponse;
  }

  private async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    const url = `${this.baseURL}${endpoint}`;
    
    try {
      // Apply request interceptors
      const config = await this.applyRequestInterceptors(options);
      
      // Make the request
      const response = await fetch(url, config);
      
      // Apply response interceptors
      const processedResponse = await this.applyResponseInterceptors(response);
      
      // Handle empty responses (like DELETE operations)
      const contentType = processedResponse.headers.get('content-type');
      if (!contentType || !contentType.includes('application/json')) {
        return undefined as T;
      }

      // Parse JSON response
      const responseData = await processedResponse.json();
      
      // Handle wrapped API responses
      if (responseData && typeof responseData === 'object' && 'data' in responseData) {
        const apiResponse = responseData as ApiResponse<T>;
        return apiResponse.data;
      }
      
      // Return direct response if not wrapped
      return responseData as T;
    } catch (error) {
      // Re-throw ApiClientError as-is
      if (error instanceof ApiClientError) {
        throw error;
      }
      
      // Wrap other errors
      console.error('API request failed:', error);
      throw new ApiClientError(
        error instanceof Error ? error.message : 'Unknown error occurred',
        'NETWORK_ERROR'
      );
    }
  }

  async get<T>(endpoint: string, params?: Record<string, string | number | boolean | undefined>): Promise<T> {
    let url = endpoint;
    if (params) {
      const searchParams = new URLSearchParams();
      Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
          searchParams.append(key, String(value));
        }
      });
      const queryString = searchParams.toString();
      if (queryString) {
        url += `?${queryString}`;
      }
    }
    return this.request<T>(url, { method: 'GET' });
  }

  async post<T>(endpoint: string, data?: unknown): Promise<T> {
    return this.request<T>(endpoint, {
      method: 'POST',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  async put<T>(endpoint: string, data: unknown): Promise<T> {
    return this.request<T>(endpoint, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  async delete<T>(endpoint: string): Promise<T> {
    return this.request<T>(endpoint, { method: 'DELETE' });
  }
}

export const apiClient = new ApiClient(API_BASE_URL);