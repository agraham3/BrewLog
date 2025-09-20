import { apiClient } from './api';
import type { 
  BrewingEquipmentResponseDto, 
  CreateBrewingEquipmentDto, 
  UpdateBrewingEquipmentDto,
  BrewingEquipmentFilterDto 
} from '@/types';

export const brewingEquipmentService = {
  getAll: (filter?: BrewingEquipmentFilterDto): Promise<BrewingEquipmentResponseDto[]> => {
    return apiClient.get<BrewingEquipmentResponseDto[]>('/equipment', filter);
  },

  getById: (id: number): Promise<BrewingEquipmentResponseDto> => {
    return apiClient.get<BrewingEquipmentResponseDto>(`/equipment/${id}`);
  },

  create: (data: CreateBrewingEquipmentDto): Promise<BrewingEquipmentResponseDto> => {
    return apiClient.post<BrewingEquipmentResponseDto>('/equipment', data);
  },

  update: (id: number, data: UpdateBrewingEquipmentDto): Promise<BrewingEquipmentResponseDto> => {
    return apiClient.put<BrewingEquipmentResponseDto>(`/equipment/${id}`, data);
  },

  delete: (id: number): Promise<void> => {
    return apiClient.delete<void>(`/equipment/${id}`);
  },
};