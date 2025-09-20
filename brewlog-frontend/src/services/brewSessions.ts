import { apiClient } from './api';
import type { 
  BrewSessionResponseDto, 
  CreateBrewSessionDto, 
  UpdateBrewSessionDto,
  BrewSessionFilterDto 
} from '@/types';

export const brewSessionService = {
  getAll: (filter?: BrewSessionFilterDto): Promise<BrewSessionResponseDto[]> => {
    return apiClient.get<BrewSessionResponseDto[]>('/brewsessions', filter);
  },

  getById: (id: number): Promise<BrewSessionResponseDto> => {
    return apiClient.get<BrewSessionResponseDto>(`/brewsessions/${id}`);
  },

  create: (data: CreateBrewSessionDto): Promise<BrewSessionResponseDto> => {
    return apiClient.post<BrewSessionResponseDto>('/brewsessions', data);
  },

  update: (id: number, data: UpdateBrewSessionDto): Promise<BrewSessionResponseDto> => {
    return apiClient.put<BrewSessionResponseDto>(`/brewsessions/${id}`, data);
  },

  delete: (id: number): Promise<void> => {
    return apiClient.delete<void>(`/brewsessions/${id}`);
  },

  toggleFavorite: (id: number): Promise<BrewSessionResponseDto> => {
    return apiClient.post<BrewSessionResponseDto>(`/brewsessions/${id}/favorite`);
  },
};