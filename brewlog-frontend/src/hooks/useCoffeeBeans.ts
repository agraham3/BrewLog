import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { coffeeBeanService } from '@/services';
import type { 
  CoffeeBeanResponseDto, 
  CreateCoffeeBeanDto, 
  UpdateCoffeeBeanDto,
  CoffeeBeanFilterDto 
} from '@/types';

// Query keys
export const coffeeBeanKeys = {
  all: ['coffeeBeans'] as const,
  lists: () => [...coffeeBeanKeys.all, 'list'] as const,
  list: (filter?: CoffeeBeanFilterDto) => [...coffeeBeanKeys.lists(), filter] as const,
  details: () => [...coffeeBeanKeys.all, 'detail'] as const,
  detail: (id: number) => [...coffeeBeanKeys.details(), id] as const,
};

// Hooks
export const useCoffeeBeans = (filter?: CoffeeBeanFilterDto) => {
  return useQuery({
    queryKey: coffeeBeanKeys.list(filter),
    queryFn: () => coffeeBeanService.getAll(filter),
    staleTime: 1000 * 60 * 5, // 5 minutes
  });
};

export const useCoffeeBean = (id: number) => {
  return useQuery({
    queryKey: coffeeBeanKeys.detail(id),
    queryFn: () => coffeeBeanService.getById(id),
    enabled: !!id,
  });
};

export const useCreateCoffeeBean = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (data: CreateCoffeeBeanDto) => coffeeBeanService.create(data),
    onSuccess: (newBean) => {
      // Invalidate and refetch coffee beans list
      queryClient.invalidateQueries({ queryKey: coffeeBeanKeys.lists() });
      
      // Add the new bean to the cache
      queryClient.setQueryData(coffeeBeanKeys.detail(newBean.id), newBean);
    },
  });
};

export const useUpdateCoffeeBean = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateCoffeeBeanDto }) => 
      coffeeBeanService.update(id, data),
    onSuccess: (updatedBean) => {
      // Invalidate and refetch coffee beans list
      queryClient.invalidateQueries({ queryKey: coffeeBeanKeys.lists() });
      
      // Update the specific bean in the cache
      queryClient.setQueryData(coffeeBeanKeys.detail(updatedBean.id), updatedBean);
    },
  });
};

export const useDeleteCoffeeBean = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (id: number) => coffeeBeanService.delete(id),
    onSuccess: (_, deletedId) => {
      // Invalidate and refetch coffee beans list
      queryClient.invalidateQueries({ queryKey: coffeeBeanKeys.lists() });
      
      // Remove the deleted bean from the cache
      queryClient.removeQueries({ queryKey: coffeeBeanKeys.detail(deletedId) });
    },
  });
};