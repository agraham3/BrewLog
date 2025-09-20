import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { brewSessionService } from '@/services';
import type { 
  BrewSessionResponseDto, 
  CreateBrewSessionDto, 
  UpdateBrewSessionDto,
  BrewSessionFilterDto 
} from '@/types';

// Query keys
export const brewSessionKeys = {
  all: ['brewSessions'] as const,
  lists: () => [...brewSessionKeys.all, 'list'] as const,
  list: (filter?: BrewSessionFilterDto) => [...brewSessionKeys.lists(), filter] as const,
  details: () => [...brewSessionKeys.all, 'detail'] as const,
  detail: (id: number) => [...brewSessionKeys.details(), id] as const,
};

// Hooks
export const useBrewSessions = (filter?: BrewSessionFilterDto) => {
  return useQuery({
    queryKey: brewSessionKeys.list(filter),
    queryFn: () => brewSessionService.getAll(filter),
    staleTime: 1000 * 60 * 2, // 2 minutes (shorter for more dynamic data)
  });
};

export const useBrewSession = (id: number) => {
  return useQuery({
    queryKey: brewSessionKeys.detail(id),
    queryFn: () => brewSessionService.getById(id),
    enabled: !!id,
  });
};

export const useCreateBrewSession = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (data: CreateBrewSessionDto) => brewSessionService.create(data),
    onSuccess: (newSession) => {
      // Invalidate and refetch brew sessions list
      queryClient.invalidateQueries({ queryKey: brewSessionKeys.lists() });
      
      // Add the new session to the cache
      queryClient.setQueryData(brewSessionKeys.detail(newSession.id), newSession);
      
      // Also invalidate analytics since new session affects stats
      queryClient.invalidateQueries({ queryKey: ['analytics'] });
    },
  });
};

export const useUpdateBrewSession = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateBrewSessionDto }) => 
      brewSessionService.update(id, data),
    onSuccess: (updatedSession) => {
      // Invalidate and refetch brew sessions list
      queryClient.invalidateQueries({ queryKey: brewSessionKeys.lists() });
      
      // Update the specific session in the cache
      queryClient.setQueryData(brewSessionKeys.detail(updatedSession.id), updatedSession);
      
      // Also invalidate analytics since session update affects stats
      queryClient.invalidateQueries({ queryKey: ['analytics'] });
    },
  });
};

export const useDeleteBrewSession = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (id: number) => brewSessionService.delete(id),
    onSuccess: (_, deletedId) => {
      // Invalidate and refetch brew sessions list
      queryClient.invalidateQueries({ queryKey: brewSessionKeys.lists() });
      
      // Remove the deleted session from the cache
      queryClient.removeQueries({ queryKey: brewSessionKeys.detail(deletedId) });
      
      // Also invalidate analytics since session deletion affects stats
      queryClient.invalidateQueries({ queryKey: ['analytics'] });
    },
  });
};

export const useToggleFavoriteBrewSession = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (id: number) => brewSessionService.toggleFavorite(id),
    onSuccess: (updatedSession) => {
      // Invalidate and refetch brew sessions list
      queryClient.invalidateQueries({ queryKey: brewSessionKeys.lists() });
      
      // Update the specific session in the cache
      queryClient.setQueryData(brewSessionKeys.detail(updatedSession.id), updatedSession);
      
      // Also invalidate analytics since favorite status affects stats
      queryClient.invalidateQueries({ queryKey: ['analytics'] });
    },
  });
};