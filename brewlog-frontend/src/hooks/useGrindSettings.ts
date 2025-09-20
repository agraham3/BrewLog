import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { grindSettingService } from '@/services';
import type { 
  GrindSettingResponseDto, 
  CreateGrindSettingDto, 
  UpdateGrindSettingDto,
  GrindSettingFilterDto 
} from '@/types';

// Query keys
export const grindSettingKeys = {
  all: ['grindSettings'] as const,
  lists: () => [...grindSettingKeys.all, 'list'] as const,
  list: (filter?: GrindSettingFilterDto) => [...grindSettingKeys.lists(), filter] as const,
  details: () => [...grindSettingKeys.all, 'detail'] as const,
  detail: (id: number) => [...grindSettingKeys.details(), id] as const,
};

// Hooks
export const useGrindSettings = (filter?: GrindSettingFilterDto) => {
  return useQuery({
    queryKey: grindSettingKeys.list(filter),
    queryFn: () => grindSettingService.getAll(filter),
    staleTime: 1000 * 60 * 5, // 5 minutes
  });
};

export const useGrindSetting = (id: number) => {
  return useQuery({
    queryKey: grindSettingKeys.detail(id),
    queryFn: () => grindSettingService.getById(id),
    enabled: !!id,
  });
};

export const useCreateGrindSetting = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (data: CreateGrindSettingDto) => grindSettingService.create(data),
    onSuccess: (newSetting) => {
      // Invalidate and refetch grind settings list
      queryClient.invalidateQueries({ queryKey: grindSettingKeys.lists() });
      
      // Add the new setting to the cache
      queryClient.setQueryData(grindSettingKeys.detail(newSetting.id), newSetting);
    },
  });
};

export const useUpdateGrindSetting = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateGrindSettingDto }) => 
      grindSettingService.update(id, data),
    onSuccess: (updatedSetting) => {
      // Invalidate and refetch grind settings list
      queryClient.invalidateQueries({ queryKey: grindSettingKeys.lists() });
      
      // Update the specific setting in the cache
      queryClient.setQueryData(grindSettingKeys.detail(updatedSetting.id), updatedSetting);
    },
  });
};

export const useDeleteGrindSetting = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (id: number) => grindSettingService.delete(id),
    onSuccess: (_, deletedId) => {
      // Invalidate and refetch grind settings list
      queryClient.invalidateQueries({ queryKey: grindSettingKeys.lists() });
      
      // Remove the deleted setting from the cache
      queryClient.removeQueries({ queryKey: grindSettingKeys.detail(deletedId) });
    },
  });
};