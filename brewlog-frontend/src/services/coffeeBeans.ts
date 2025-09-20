import { apiClient } from './api';
import type { 
  CoffeeBeanResponseDto, 
  CreateCoffeeBeanDto, 
  UpdateCoffeeBeanDto,
  CoffeeBeanFilterDto 
} from '@/types';

export const coffeeBeanService = {
  getAll: (filter?: CoffeeBeanFilterDto): Promise<CoffeeBeanResponseDto[]> => {
    return apiClient.get<CoffeeBeanResponseDto[]>('/coffeebeans', filter);
  },

  getById: (id: number): Promise<CoffeeBeanResponseDto> => {
    return apiClient.get<CoffeeBeanResponseDto>(`/coffeebeans/${id}`);
  },

  create: (data: CreateCoffeeBeanDto): Promise<CoffeeBeanResponseDto> => {
    return apiClient.post<CoffeeBeanResponseDto>('/coffeebeans', data);
  },

  update: (id: number, data: UpdateCoffeeBeanDto): Promise<CoffeeBeanResponseDto> => {
    return apiClient.put<CoffeeBeanResponseDto>(`/coffeebeans/${id}`, data);
  },

  delete: (id: number): Promise<void> => {
    return apiClient.delete<void>(`/coffeebeans/${id}`);
  },
};