import { apiClient } from './api';
import type { 
  CoffeeBean, 
  CreateCoffeeBeanDto, 
  UpdateCoffeeBeanDto,
  CoffeeBeanFilter 
} from '@/types';

export const coffeeBeanService = {
  getAll: (filter?: CoffeeBeanFilter): Promise<CoffeeBean[]> => {
    const params = new URLSearchParams();
    if (filter?.search) params.append('search', filter.search);
    if (filter?.roastLevel) params.append('roastLevel', filter.roastLevel);
    if (filter?.brand) params.append('brand', filter.brand);
    
    const queryString = params.toString();
    return apiClient.get<CoffeeBean[]>(`/coffeebeans${queryString ? `?${queryString}` : ''}`);
  },

  getById: (id: number): Promise<CoffeeBean> => {
    return apiClient.get<CoffeeBean>(`/coffeebeans/${id}`);
  },

  create: (data: CreateCoffeeBeanDto): Promise<CoffeeBean> => {
    return apiClient.post<CoffeeBean>('/coffeebeans', data);
  },

  update: (id: number, data: UpdateCoffeeBeanDto): Promise<CoffeeBean> => {
    return apiClient.put<CoffeeBean>(`/coffeebeans/${id}`, data);
  },

  delete: (id: number): Promise<void> => {
    return apiClient.delete<void>(`/coffeebeans/${id}`);
  },
};