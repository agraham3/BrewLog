import { apiClient } from './api';
import type { 
  DashboardStatsDto,
  CorrelationAnalysisDto,
  RecommendationDto,
  EquipmentPerformanceDto
} from '@/types';

export const analyticsService = {
  getDashboardStats: (): Promise<DashboardStatsDto> => {
    return apiClient.get<DashboardStatsDto>('/analytics/dashboard');
  },

  getCorrelationAnalysis: (): Promise<CorrelationAnalysisDto> => {
    return apiClient.get<CorrelationAnalysisDto>('/analytics/correlations');
  },

  getRecommendations: (): Promise<RecommendationDto[]> => {
    return apiClient.get<RecommendationDto[]>('/analytics/recommendations');
  },

  getEquipmentPerformance: (): Promise<EquipmentPerformanceDto> => {
    return apiClient.get<EquipmentPerformanceDto>('/analytics/equipment-performance');
  },
};