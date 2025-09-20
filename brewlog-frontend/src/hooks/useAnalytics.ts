import { useQuery } from '@tanstack/react-query';
import { analyticsService } from '@/services';
import type { 
  DashboardStatsDto,
  CorrelationAnalysisDto,
  RecommendationDto,
  EquipmentPerformanceDto
} from '@/types';

// Query keys
export const analyticsKeys = {
  all: ['analytics'] as const,
  dashboard: () => [...analyticsKeys.all, 'dashboard'] as const,
  correlations: () => [...analyticsKeys.all, 'correlations'] as const,
  recommendations: () => [...analyticsKeys.all, 'recommendations'] as const,
  equipmentPerformance: () => [...analyticsKeys.all, 'equipmentPerformance'] as const,
};

// Hooks
export const useDashboardStats = () => {
  return useQuery({
    queryKey: analyticsKeys.dashboard(),
    queryFn: () => analyticsService.getDashboardStats(),
    staleTime: 1000 * 60 * 10, // 10 minutes (analytics can be cached longer)
    refetchOnWindowFocus: false,
  });
};

export const useCorrelationAnalysis = () => {
  return useQuery({
    queryKey: analyticsKeys.correlations(),
    queryFn: () => analyticsService.getCorrelationAnalysis(),
    staleTime: 1000 * 60 * 15, // 15 minutes
    refetchOnWindowFocus: false,
  });
};

export const useRecommendations = () => {
  return useQuery({
    queryKey: analyticsKeys.recommendations(),
    queryFn: () => analyticsService.getRecommendations(),
    staleTime: 1000 * 60 * 30, // 30 minutes (recommendations change less frequently)
    refetchOnWindowFocus: false,
  });
};

export const useEquipmentPerformance = () => {
  return useQuery({
    queryKey: analyticsKeys.equipmentPerformance(),
    queryFn: () => analyticsService.getEquipmentPerformance(),
    staleTime: 1000 * 60 * 15, // 15 minutes
    refetchOnWindowFocus: false,
  });
};