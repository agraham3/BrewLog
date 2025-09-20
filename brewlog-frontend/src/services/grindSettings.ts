import { apiClient } from './api';
import type { 
  GrindSettingResponseDto, 
  CreateGrindSettingDto, 
  UpdateGrindSettingDto,
  GrindSettingFilterDto 
} from '@/types';

export const grindSettingService = {
  getAll: (filter?: GrindSettingFilterDto): Promise<GrindSettingResponseDto[]> => {
    return apiClient.get<GrindSettingResponseDto[]>('/grindsettings', filter);
  },

  getById: (id: number): Promise<GrindSettingResponseDto> => {
    return apiClient.get<GrindSettingResponseDto>(`/grindsettings/${id}`);
  },

  create: (data: CreateGrindSettingDto): Promise<GrindSettingResponseDto> => {
    return apiClient.post<GrindSettingResponseDto>('/grindsettings', data);
  },

  update: (id: number, data: UpdateGrindSettingDto): Promise<GrindSettingResponseDto> => {
    return apiClient.put<GrindSettingResponseDto>(`/grindsettings/${id}`, data);
  },

  delete: (id: number): Promise<void> => {
    return apiClient.delete<void>(`/grindsettings/${id}`);
  },
};