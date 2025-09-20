import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { brewingEquipmentService } from '@/services';
import type { 
  BrewingEquipmentResponseDto, 
  CreateBrewingEquipmentDto, 
  UpdateBrewingEquipmentDto,
  BrewingEquipmentFilterDto 
} from '@/types';

// Query keys
export const brewingEquipmentKeys = {
  all: ['brewingEquipment'] as const,
  lists: () => [...brewingEquipmentKeys.all, 'list'] as const,
  list: (filter?: BrewingEquipmentFilterDto) => [...brewingEquipmentKeys.lists(), filter] as const,
  details: () => [...brewingEquipmentKeys.all, 'detail'] as const,
  detail: (id: number) => [...brewingEquipmentKeys.details(), id] as const,
};

// Hooks
export const useBrewingEquipment = (filter?: BrewingEquipmentFilterDto) => {
  return useQuery({
    queryKey: brewingEquipmentKeys.list(filter),
    queryFn: () => brewingEquipmentService.getAll(filter),
    staleTime: 1000 * 60 * 5, // 5 minutes
  });
};

export const useBrewingEquipmentItem = (id: number) => {
  return useQuery({
    queryKey: brewingEquipmentKeys.detail(id),
    queryFn: () => brewingEquipmentService.getById(id),
    enabled: !!id,
  });
};

export const useCreateBrewingEquipment = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (data: CreateBrewingEquipmentDto) => brewingEquipmentService.create(data),
    onSuccess: (newEquipment) => {
      // Invalidate and refetch equipment list
      queryClient.invalidateQueries({ queryKey: brewingEquipmentKeys.lists() });
      
      // Add the new equipment to the cache
      queryClient.setQueryData(brewingEquipmentKeys.detail(newEquipment.id), newEquipment);
    },
  });
};

export const useUpdateBrewingEquipment = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateBrewingEquipmentDto }) => 
      brewingEquipmentService.update(id, data),
    onSuccess: (updatedEquipment) => {
      // Invalidate and refetch equipment list
      queryClient.invalidateQueries({ queryKey: brewingEquipmentKeys.lists() });
      
      // Update the specific equipment in the cache
      queryClient.setQueryData(brewingEquipmentKeys.detail(updatedEquipment.id), updatedEquipment);
    },
  });
};

export const useDeleteBrewingEquipment = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (id: number) => brewingEquipmentService.delete(id),
    onSuccess: (_, deletedId) => {
      // Invalidate and refetch equipment list
      queryClient.invalidateQueries({ queryKey: brewingEquipmentKeys.lists() });
      
      // Remove the deleted equipment from the cache
      queryClient.removeQueries({ queryKey: brewingEquipmentKeys.detail(deletedId) });
    },
  });
};